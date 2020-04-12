using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using Xceed.Zip;

// TODO : 1. Get delete archive working properly
//        2. Due to the Xcedd.Zip has limitation to different zip extention format, such as PKZIP, RAR, CAB, SFX(self-extracting exe), JAR etc.
//           explor other zip vender, such as 7Zip to support all type of zip files -Henry Li
namespace McAfeeLabs.Engineering.Automation.Base
{
    abstract public class ZipFileInspectorReceiver : FolderInspectorReceiver, IDisposable
    {
        #region Declarations

        protected static object _syncObj = new object();
        protected string _extractingSourceDir;
        protected string _extractingResultsDir;
        protected string _password;
        protected string _lastErrorMessage;
        protected Stack<string> _unpackedFolderstack = new Stack<string>();
        protected Dictionary<string, int> _recuringdictionary = new Dictionary<string, int>();
        protected List<ZipFileInspectorReceiver> _childList = new List<ZipFileInspectorReceiver>();

        #endregion

        #region Constructors

        public ZipFileInspectorReceiver( IEnumerable<string> sourceFolders,
                                         int batchSize = 5,
                                         bool recursive = true,
                                         string password = null)
            :base(sourceFolders, batchSize, recursive)
        {
            _unpackedFolderstack.Push(_extractingSourceDir);
            _unpackedFolderstack.Push(_extractingResultsDir);
            _password = string.IsNullOrEmpty(password) ? GlobalDefinitions.DefaultInfectedPassword : password;
            Xceed.Zip.Licenser.LicenseKey = GlobalDefinitions.XceedZipLicenseKey;
        }

        #endregion

        #region Properties

        public bool? ThrowException { get; set; }
        public string LastErrorMessage { get { return _lastErrorMessage; } }
        public string ResultsFolder { get { return _extractingResultsDir; } }

        #endregion

        #region Public Methods

        override public void Dispose()
        {
            try
            {
                while (_unpackedFolderstack.Count > 0)
                {
                    var folder = _unpackedFolderstack.Pop();
                    if (Directory.Exists(folder))
                        Directory.Delete(folder, true);
                }
            }
            catch (Exception ex)
            {
                if (null != ThrowException && ThrowException.Value == true)
                    throw ex;
            }
        }

        #endregion

        #region Propeted Meghods

        protected string ExtractContents(FileInfo fileInfo)
        {
            var freeDiskSpace = fileInfo.Directory.Root.Name.GetFreeDiskSpace();
            if (3.0 * fileInfo.Length > freeDiskSpace)
                throw new ApplicationException(string.Format(@"--- The current free disk space <{0}> of drive <{1}> is not enough to extracting the contents!", freeDiskSpace, fileInfo.Directory.Root));
            
            var extractedFolder = Path.Combine(_extractingResultsDir, fileInfo.MD5().ToString());
            CreateSubfolder(extractedFolder, fileInfo.Name);

            fileInfo.FullName.RemoveFileReadOnlyAttribute();
            QuickZip.Unzip(fileInfo.FullName, extractedFolder, _password, true, true, true, "*");
            //File.Delete(fileInfo.FullName);

            return extractedFolder;
        }

        protected void CreateSubfolder(string parentFolder, string subfolder)
        {
            if (!Directory.Exists(parentFolder))
            {
                Directory.CreateDirectory(parentFolder);
                _unpackedFolderstack.Push(parentFolder);
            }
            if (!_recuringdictionary.ContainsKey(subfolder))
                _recuringdictionary.Add(subfolder, 1);
            else
            {
                int count;
                _recuringdictionary.TryGetValue(subfolder, out count);
                _recuringdictionary.Remove(subfolder);
                _recuringdictionary.Add(subfolder, count + 1);
            }
        }

        override protected void InitializeReceiver(IEnumerable<string> sourceFolders, int batchSize, bool recursive)
        {
            List<string> walkSourceFolderList = new List<string>();

            var typeName = typeof(ZipFileInspectorReceiver).Name;
            CreateWalkDirectory(typeName, @"Source", ref _extractingSourceDir);
            CreateWalkDirectory(typeName, @"Results", ref _extractingResultsDir);

            sourceFolders.ToList().ForEach(x =>
            {
                var sourceZipFile = x.ResolveSingleFilePath();
                if (!string.IsNullOrEmpty(sourceZipFile)
                    && File.Exists(sourceZipFile))
                {
                    var fileInfo = new FileInfo(sourceZipFile.TrimLongPathPrefix());
                    File.Copy(sourceZipFile, Path.Combine( _extractingSourceDir, 
                                                           fileInfo.Name), 
                                                           true);
                    if (!walkSourceFolderList.Contains(_extractingSourceDir))
                        walkSourceFolderList.Add(_extractingSourceDir);
                }
                else if(Directory.Exists(x) && !walkSourceFolderList.Contains(x))
                    walkSourceFolderList.Add(x);

            });

            _folderInspector = new ZipFileFolderInspector(walkSourceFolderList, recursive) { BatchSize = batchSize };
            _folderInspector.BatchResults += OnFileInspectBatchResults;
        }

        private void CreateWalkDirectory( string typeName, 
                                          string suffix, 
                                          ref string dir)
        {
            dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, typeName + suffix);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        abstract protected ZipFileInspectorReceiver InspectorReceiverFactory(string folderName);
        #endregion
    }
}
