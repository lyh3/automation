using System;
using System.Collections.Generic;
using System.Linq;
using Delimon.Win32.IO;

using log4net;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [Serializable]
    abstract public class FolderInspectorBase : IDisposable
    {
        #region Declarations

        protected static object _syncObj = new object();
        protected IEnumerable<string> _folders;
        protected int _batchSize = 10;
        protected bool _isComplete = false;
        protected bool _recurseIntoSubdirectories = true;

        [ThreadStatic]
        protected List<string> _currentFiles = new List<string>();

        private event EventHandler _eventBatchResults;

        #endregion

        #region Constructors

        public FolderInspectorBase(IEnumerable<string> folders, bool recursive = true)
            : this(folders)
        {
            _recurseIntoSubdirectories = recursive;
            MaxSize = int.MaxValue;
            Pause = false;
        }

        public FolderInspectorBase(IEnumerable<string> folders)
        {
            var dirs = new List<string>();
            foreach (var folder in folders)
            {
                var folderPath = folder.Trim();
                if (!Delimon.Win32.IO.Directory.Exists(folderPath))
                    throw new ArgumentException(string.Format(@"--- The folder <{0}> is not exists, invalid or is file path", folder));
                dirs.Add(folderPath);
            }
            _folders = dirs;
        }

        #endregion

        #region Public Methods

        public IEnumerable<string> Inspect()
        {
            var files = InspectDirectories(_folders);
            _isComplete = true;

            if (null != _eventBatchResults)
                _eventBatchResults(this, null);

            return files;
        }

        public void Dispose()
        {
            _currentFiles.Clear();
            _isComplete = true;
        }

        #endregion

        #region Properties

        public event EventHandler BatchResults
        {
            add { _eventBatchResults = (EventHandler)Delegate.Combine(_eventBatchResults, value); }
            remove { _eventBatchResults = (EventHandler)Delegate.Remove(_eventBatchResults, value); }
        }

        public int MaxSize { get; set; }

        public List<string> CurrentFiles
        {
            get { return _currentFiles; }
            set
            {
                lock (_syncObj)
                {
                    _currentFiles = value;
                    if (_currentFiles.Count >= _batchSize
                       && null != _eventBatchResults)
                    {
                        RaiseBatchEvent();
                        _currentFiles.Clear();
                    }
                }
            }
        }

        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        public ILog Logger { get; set; }
        public bool IsComplete { get { return _isComplete; } }
        public bool Pause { get; set; }

        #endregion

        #region Private Methods

        protected void RaiseBatchEvent()
        {
            _eventBatchResults(this, EventArgs.Empty);
        }

        private List<string> InspectDirectories(IEnumerable<string> directoriesToSearch)
        {
            var searchOption = _recurseIntoSubdirectories ? Delimon.Win32.IO.SearchOption.AllDirectories : Delimon.Win32.IO.SearchOption.TopDirectoryOnly;

            var allFilePaths = from directory in directoriesToSearch
                               from file in GetAllFilesInDirectory(directory, searchOption)
                               select file;
            var fileDetails = from filePath in allFilePaths
                              select filePath;

            return fileDetails.ToList();
        }

        protected void GetEntries(string directoryPath,
                                  ref IEnumerable<string> files,
                                  ref IEnumerable<string> subdirectories)
        {
            try
            {
                files = Delimon.Win32.IO.Directory.GetFiles(directoryPath.Trim());
                subdirectories = Delimon.Win32.IO.Directory.GetDirectories(directoryPath.Trim());
            }
            catch (UnauthorizedAccessException)
            {
                if (null != Logger)
                    Logger.Error(string.Format("--- GetAllFilesInDirectory: No permission to access folder {0}", directoryPath));
            }
            catch (System.IO.IOException e)
            {
                if (null != Logger)
                    Logger.Error(string.Format("--- GetAllFilesInDirectory: I/O error: {0}", e.Message));
            }
            catch (Exception e)
            {
                if (null != Logger)
                    Logger.Error(string.Format("--- GetAllFilesInDirectory: Generic error: {0}", e.Message));
            }
        }

        abstract protected IEnumerable<string> GetAllFilesInDirectory(string directoryPath, Delimon.Win32.IO.SearchOption searchOption);

        #endregion
    }
}
