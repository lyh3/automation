using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class ZipWalker : ZipFileInspectorReceiver
    {
        #region Declarations

        protected IZipWalkerVisitor _visitor;
        protected int _currentDeepLevel = 0;
        protected long _totalsize = 0L;

        #endregion

        #region Constructors

        public ZipWalker(IEnumerable<string> sourceFolders)
            : base(sourceFolders)
        {
            _canDispose = false;
        }
        public ZipWalker( IEnumerable<string> sourceFolders,
                          int batchSize)
            : this(sourceFolders)
        {
            base.BatchSize = batchSize;
        }

        public ZipWalker( IZipWalkerVisitor visitor,
                          IEnumerable<string> sourceFolders,
                          int batchSize)
            : this(sourceFolders, batchSize)
        {
            _visitor = visitor;
        }

        #endregion

        #region Properties

        override public bool CanDispose
        {
            get { return _canDispose; }
            set
            {
                _canDispose = value;
                _childList.ForEach(x => x.CanDispose = value);
            }
        }

        public long TotalSize
        {
            get { return _totalsize; }
            set { _totalsize = value; }
        }

        public int CurrentDeepLevel { get { return _currentDeepLevel; } }

        #endregion

        #region Public Methods

        override public void Dispose()
        {
            try
            {
                if (Directory.Exists(_extractingSourceDir))
                    Directory.Delete(_extractingSourceDir, true);
            }catch{}
            
            if (!CanDispose) return;

            _childList.ForEach(x => x.Dispose());
            base.Dispose();
        }

        #endregion

        #region Private Methods

        override protected void OnFileInspectBatchResults(object sender, EventArgs e)
        {
            _folderInspector.CurrentFiles.ForEach(filepath =>
            {
                var extractedFolder = UpdateExtrationResults(filepath);
                if (!string.IsNullOrEmpty(extractedFolder) && Directory.Exists(extractedFolder))
                {
                    var zipWalker = InspectorReceiverFactory(extractedFolder);
                    _childList.Add(zipWalker);
                    zipWalker.Inspect();
                }
            });
        }

        virtual protected string UpdateExtrationResults(string filepath)
        {
            try
            {
                var extractedFolder = ExtractContents(new FileInfo(filepath));
                if (Directory.Exists(extractedFolder))
                {
                    _receivedFiles.Clear();
                    var currentFiles = Directory.GetFiles(extractedFolder);
                    _receivedFiles.AddRange(currentFiles);
                    VisitInPages();
                }

                _receivedFiles.ForEach(x =>
                {
                    if (File.Exists(x))
                    {
                        var fileInfo = new FileInfo(x);
                        _totalsize += fileInfo.Length;
                    }
                });
                _currentDeepLevel += 1;

                return extractedFolder;
            }
            catch(Exception ex)
            { 
                if (null != ThrowException && ThrowException.Value)
                    throw new ApplicationException(string.Format(@"--- Failed to extract <{0}>, error was: {1}", filepath, ex.Message), ex); 
            }
            return null;
        }

        private void VisitInPages()
        {
            if (null == _visitor || _receivedFiles.Count == 0) return;

            var page = 0;
            for (; ; )
            {
                var take = _receivedFiles.Page<string>(page, base.BatchSize).ToList();
                if (null == take || take.Count <= 0) break;
                _visitor.Visit(take);
                page += 1;
            }
        }

        override protected ZipFileInspectorReceiver InspectorReceiverFactory(string folderName)
        {
            return new ZipWalker(_visitor,
                                 new[] { folderName },
                                 _folderInspector.BatchSize);
        }

        #endregion
    }
}
