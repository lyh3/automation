using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;

using Ionic.Zip;
using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class ZipBomb : IDisposable
    {
        #region Declatations

        private const int MaxDeepLevel = 8;
        private const int MinDeepLevel = 3;
        private string _baseDir;
        private string _zipfilePath;
        private string _password;
        private string _lastErrorMessage;
        private long _totalsize = -1L;
        private int _bombThreshold = 5;
        private bool _isZipBomb = false;
        private long? _maxsize = null;
        private Queue<string> _unpackedFilequeue = new Queue<string>();
        private Stack<string> _unpackedFolderstack = new Stack<string>();
        private Dictionary<string, int> _recuringdictionary = new Dictionary<string, int>();

        #endregion

        #region Constructors

        public ZipBomb( string zipfilepath,
                        string extractFolderName,
                        int deepLevelThreshold,
                        string password = null,
                        long? maxsize = null,
                        bool? throwException = null)
        {
            if (string.IsNullOrEmpty(extractFolderName)) 
                throw new ArgumentException(@"---The folder for zip bomb extraction must be specified");
            if (!Directory.Exists(extractFolderName))
                Directory.CreateDirectory(extractFolderName);
            _baseDir = extractFolderName;
            _unpackedFolderstack.Push(_baseDir);
            _password = password;
            _maxsize = maxsize;
            BombThreshold = deepLevelThreshold;
            ThrowException = throwException;
            ZipFilePath = zipfilepath;
        }

        #endregion

        #region Properties

        public string LastErrorMessage { get { return _lastErrorMessage; } }
        public bool IsZipFile { get; set; }
        public bool IsZipBomb { get { return _isZipBomb; } }
        public long TotalSize { get { return _totalsize; } }
        public bool IsUnpackedSizeTooLarge { get { return (null != _maxsize && _maxsize.Value < _totalsize) ? true : false; } }

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
                    InitializeZipBomb();
                }
            }
        }

        private long ExtractedSize
        {
            get
            {
                var totalsize = -1L;

                if (_unpackedFilequeue.Count > 0)
                {
                    var itr = _unpackedFilequeue.GetEnumerator();
                    while (itr.MoveNext())
                    {
                        var file = itr.Current;
                        if (!string.IsNullOrEmpty(file) && File.Exists(file))
                        {
                            var fileInfo = new FileInfo(file);
                            totalsize += fileInfo.Length;
                        }
                    }
                }

                return totalsize;
            }
        }

        public bool? ThrowException { get; set; }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            try
            {
                while (_unpackedFilequeue.Count > 0)
                {
                    var file = _unpackedFilequeue.Dequeue();
                    if (file.RemoveFileReadOnlyAttribute() && File.Exists(file))
                        File.Delete(file);
                }
                while (_unpackedFolderstack.Count > 0)
                {
                    var folder = _unpackedFolderstack.Pop();
                    if (Directory.Exists(folder))
                        Directory.Delete(folder, true);
                }
            }
            catch( Exception ex)
            {
                if (null != ThrowException && ThrowException.Value == true)
                    throw ex;
            }
        }

        #endregion

        #region Private Methods

        private void RetrieveEntryHeirachy(string filename)
        {
            for (; null != GetZipEntry(filename, string.Empty, 0, _baseDir); ) ;
        }

        private void InitializeZipBomb()
        {
            if (!string.IsNullOrEmpty(ZipFilePath) && File.Exists(ZipFilePath))
            {
                IsZipFile = ZipFile.IsZipFile(ZipFilePath);

                if (IsZipFile)
                    RetrieveEntryHeirachy(ZipFilePath);

                Dispose();
            }
        }

        private IEnumerable<ZipEntry> GetZipEntry(string filepath,
                                                    string subfolderPreffix,
                                                    int deepLevel,
                                                    string basdir = null)
        {
            Exception exception = null;
            var TrimArray = new[] { '\\', '_' };
            if (!string.IsNullOrEmpty(filepath))
            {
                try
                {
                    using (var zipfile = ZipFile.Read(filepath))
                    {
                        zipfile.Password = _password;
                        foreach (var entry in zipfile.Entries)
                        {
                            var filename = Path.GetFileName(filepath);

                            _totalsize += entry.UncompressedSize;

                            if (null != _maxsize
                                && (_totalsize > _maxsize || ExtractedSize > _maxsize))
                            {
                                return null;
                            }

                            if (deepLevel > BombThreshold)
                            {
                                _isZipBomb = true;// IsRecuring();
                                return null;
                            }

                            var parentpath = !string.IsNullOrEmpty(basdir) ? _baseDir : filepath.Replace(filename, string.Empty).Trim(TrimArray);
                            var dir = string.Format(@"{0}\{1}{2}",
                                                    parentpath,
                                                    subfolderPreffix,
                                                    !string.IsNullOrEmpty(filename) ? filename : Guid.NewGuid().ToString());

                            CreateSubfolder(filename, dir);

                            entry.Extract(dir, ExtractExistingFileAction.OverwriteSilently);
                            var f = string.Format(@"{0}\{1}", dir, entry.FileName);
                            _unpackedFilequeue.Enqueue(f);

                            if (ZipFile.IsZipFile(f))
                                return GetZipEntry(f, subfolderPreffix + "_", deepLevel += 1);
                        }
                    }
                }
                catch (BadPasswordException bdpwdex)
                {
                    _lastErrorMessage = string.Format(@"The zip file <{0}> is pass word protected, a correct pass word must be provided.", filepath);
                    exception = bdpwdex;
                }
                catch (BadReadException rdex)
                {
                    _lastErrorMessage = string.Format(@"Exception caught when read zip file <{0}>, error was:{1}.", filepath, null != rdex.InnerException ? rdex.InnerException.Message: rdex.Message);
                    exception = rdex;
                }
                catch (Exception ex)
                {
                    _lastErrorMessage = ex.Message;
                    exception = ex;
                }

                if (null != exception)
                {
                    Dispose();
                    if( null != ThrowException && ThrowException.Value == true)
                        throw exception;
                }
            }
            return null;
        }

        private void CreateSubfolder(string filename, string dir)
        {
            dir = dir.Replace("\\___", string.Empty);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                if (!_recuringdictionary.ContainsKey(filename))
                    _recuringdictionary.Add(filename, 1);
                else
                {
                    int count;
                    _recuringdictionary.TryGetValue(filename, out count);
                    _recuringdictionary.Remove(filename);
                    _recuringdictionary.Add(filename, count + 1);
                }
                _unpackedFolderstack.Push(dir);
            }
        }

        private bool IsRecuring()
        {
            var recuringcount = 0;

            var itr = _recuringdictionary.GetEnumerator();

            while (itr.MoveNext())
            {
                if (itr.Current.Value > recuringcount)
                    recuringcount = itr.Current.Value;
            }

            return recuringcount > 2;
        }

        #endregion
    }
}
