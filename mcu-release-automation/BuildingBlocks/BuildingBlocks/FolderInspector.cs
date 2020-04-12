using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using log4net;

namespace Automation.Base.BuildingBlocks
{
    public class FolderInspector
    {
        #region Declarations

        static object _syncObj = new object();
        private IEnumerable<string> _folders;
        private int _batchSize = 10;
        private bool _isComplete = false;
        private bool _recurseIntoSubdirectories = true;

        [ThreadStatic]
        private List<string> _currentFiles = new List<string>();

        private event EventHandler _eventBatchResults;

        #endregion

        #region Constructor

        public FolderInspector(IEnumerable<string> folders, bool recursive = true)
            : this(folders)
        {
            _recurseIntoSubdirectories = recursive;
        }

        public FolderInspector(IEnumerable<string> folders)
        {
            foreach (var folder in folders)
            {
                if (!Delimon.Win32.IO.Directory.Exists(folder))
                    throw new ArgumentException(string.Format(@"--- The folder <{0}> is not exists, invalid or is file path", folder));
            }
            _folders = folders;
        }

        #endregion

        #region Public Methods

        public IEnumerable<string> Incpect()
        {
            var files = InspectDirectories(_folders);
            _isComplete = true;

            if (null != _eventBatchResults)
                _eventBatchResults(this, null);

            return files;
        }

        #endregion

        #region Properties

        public event EventHandler BatchResults
        {
            add { _eventBatchResults = (EventHandler)Delegate.Combine(_eventBatchResults, value); }
            remove { _eventBatchResults = (EventHandler)Delegate.Remove(_eventBatchResults, value); }
        }

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
                        _eventBatchResults(this, null);
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

        #endregion

        #region Private Methods

        private List<string> InspectDirectories(IEnumerable<string> directoriesToSearch)
        {
            var searchOption = _recurseIntoSubdirectories ? Delimon.Win32.IO.SearchOption.AllDirectories : Delimon.Win32.IO.SearchOption.TopDirectoryOnly;

            var allFilePaths = from directory in directoriesToSearch
                               from file in GetAllFilesInDirectory(directory.LongPathFormat(), searchOption)
                               select file;
            var fileDetails = from filePath in allFilePaths
                              select filePath;

            return fileDetails.ToList();
        }

        private IEnumerable<string> GetAllFilesInDirectory(string directoryPath, Delimon.Win32.IO.SearchOption searchOption)
        {
            IEnumerable<string> files = null;
            IEnumerable<string> subdirectories = null;
            try
            {
                files = Directory.EnumerateFiles(directoryPath.LongPathFormat());
                subdirectories = Directory.EnumerateDirectories(directoryPath.LongPathFormat());
            }
            catch (UnauthorizedAccessException)
            {
                if (null != Logger)
                    Logger.Error(string.Format("--- GetAllFilesInDirectory: No permission to access folder {0}", directoryPath));
            }
            catch (IOException e)
            {
                if (null != Logger)
                    Logger.Error(string.Format("--- GetAllFilesInDirectory: I/O error: {0}", e.Message));
            }

            if (files != null)
            {
                var f = _currentFiles;
                foreach (string file in files)
                {
                    f.Add(file.LongPathFormat());
                    CurrentFiles = f;
                    yield return file;
                }
            }

            if (subdirectories != null && searchOption == Delimon.Win32.IO.SearchOption.AllDirectories)
            {
                foreach (string subdirectory in subdirectories)
                {
                    if ((Delimon.Win32.IO.File.GetAttributes(subdirectory) & Delimon.Win32.IO.FileAttributes.ReparsePoint) != Delimon.Win32.IO.FileAttributes.ReparsePoint)
                    {
                        foreach (string file in GetAllFilesInDirectory(subdirectory, searchOption))
                        {
                            CurrentFiles.Add(file.LongPathFormat());
                            yield return file.LongPathFormat();
                        }
                    }
                    else if (null != Logger)
                        Logger.Warn(string.Format("{0} --- JUNCTION detected, skipped", subdirectory));
                }
            }
        }

        #endregion
    }
}
