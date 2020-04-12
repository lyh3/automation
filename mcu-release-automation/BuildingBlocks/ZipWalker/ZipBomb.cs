using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;

using Ionic.Zip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class ZipBomb : ZipWalker
    {
        #region Declatations

        private const int MaxDeepLevel = 2000;
        private const int MinDeepLevel = 3;
        private string _zipfilePath;
        private bool _isZipBomb = false;
        private long? _maxsize = null;
        private int _bombThreshold = 5;
        private ZipBomb _parentInspectorReceiver;

        #endregion

        #region Constructors

        public ZipBomb(IEnumerable<string> sourceFolders)
            : base(sourceFolders) { }

        public ZipBomb( string zipfilepath,
                        int deepLevelThreshold,
                        string password = null,
                        long? maxsize = long.MaxValue,
                        bool? throwException = null)
            : this(new[] { zipfilepath })
        {
            _password = string.IsNullOrEmpty(password) ? GlobalDefinitions.DefaultInfectedPassword : password;
            _maxsize = maxsize;
            BombThreshold = deepLevelThreshold;
            ThrowException = throwException;
            ZipFilePath = zipfilepath;
            ZipProviderContainer.Instance.ZipProvider.SourcArchive = zipfilepath;
            IsZipFile = ZipProviderContainer.Instance.ZipProvider.IsArchive;
        }

        #endregion

        #region Properties

        public ZipBomb ParentInspectorReceiver 
        {
            get { return _parentInspectorReceiver; }
            set { _parentInspectorReceiver = value; }
        }
        public bool IsZipBomb 
        { 
            get { return _isZipBomb; }
            set
            {
                _isZipBomb = value;
                if (null != _parentInspectorReceiver)
                    _parentInspectorReceiver.IsZipBomb = value;
            }
        }

        public bool IsZipFile { get; set; }

        public bool IsUnpackedSizeTooLarge { get { return (null != _maxsize && _maxsize.Value < _totalsize) ? true : false; } }
        public int CurrentDeepLevel
        {
            get { return _currentDeepLevel; }
            set { _currentDeepLevel = value; }
        }

        public int BombThreshold
        {
            get { return _bombThreshold; }
            set
            {
                if (value < MinDeepLevel || value > MaxDeepLevel)
                {
                    Dispose();
                    throw new ArgumentException(string.Format(@"--- The input value <{0}> is not in the range of [{1}] - {2}]", value, MinDeepLevel, MaxDeepLevel));
                }
                _bombThreshold = value;
            }
        }

        public string ZipFilePath
        {
            get { return _zipfilePath; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != _zipfilePath)
                {
                    var exception = value.TryDemandFullAccess();
                    if (null != exception) throw exception;

                    _zipfilePath = value;
                }
            }
        }

        #endregion

        #region Private Methods

        override protected void OnFileInspectBatchResults(object sender, EventArgs e)
        {
            _folderInspector.CurrentFiles.ForEach(filepath =>
            {
                var extractedFolder = UpdateExtrationResults(filepath);
                if (_isZipBomb)
                    return;

                var zipBomb = InspectorReceiverFactory(extractedFolder);

                _childList.Add(zipBomb);
                zipBomb.Inspect();
            });
        }

        override protected string UpdateExtrationResults(string filepath)
        {
            if (string.IsNullOrEmpty(filepath)) return null;

            var extractedFolder = base.UpdateExtrationResults(filepath);

            var folderName = Path.GetDirectoryName(extractedFolder);
            if (!string.IsNullOrEmpty(folderName))
            {
                if (!_recuringdictionary.ContainsKey(folderName))
                    _recuringdictionary.Add(folderName, 1);
                else
                {
                    int count;
                    _recuringdictionary.TryGetValue(folderName, out count);
                    _recuringdictionary.Remove(folderName);
                    _recuringdictionary.Add(folderName, count + 1);
                }

            }
            if ((null != _maxsize && _totalsize > _maxsize.Value)
                || _currentDeepLevel > BombThreshold)
                IsZipBomb = true;

            return extractedFolder;
        }


        override protected ZipFileInspectorReceiver InspectorReceiverFactory(string folderName)
        {
            var zipBomb = new ZipBomb(folderName,
                                      BombThreshold,
                                      _password,
                                      null != _maxsize ? _maxsize.Value : long.MaxValue,
                                      null != ThrowException ? ThrowException.Value : false)
            {
                CurrentDeepLevel = _currentDeepLevel,
                TotalSize = _totalsize,
                ParentInspectorReceiver = this
            };
            return zipBomb;
        }
        
        #endregion
    }
}
