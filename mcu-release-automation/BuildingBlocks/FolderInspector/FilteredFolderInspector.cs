using System.Collections.Generic;
using System.IO;

using log4net;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class FilteredFolderInspector : FolderInspectorBase
    {
         #region Declarations

        private List<FileDetails> _fileExclusions = new List<FileDetails>();
        private int _yieldCount = 0;

        #endregion
        
        #region Constructor

        public FilteredFolderInspector( IEnumerable<string> folders, 
                                        int batchSize,
                                        bool recursive = true,
                                        List<string> filterList = null,
                                        int returnCount = int.MaxValue,
                                        int maxSize = int.MaxValue)
            : this(folders, recursive)
        {
            FilterList = new List<string>();
            FilterList.AddRange(filterList);
            ReturnCount = returnCount;
            MaxSize = maxSize;
        }

        public FilteredFolderInspector(IEnumerable<string> folders, bool recursive = true)
            : this(folders)
        {
            base._recurseIntoSubdirectories = recursive;
        }

        public FilteredFolderInspector(IEnumerable<string> folders)
            : base(folders)
        {
        }

        #endregion

        #region Properties

        public int ReturnCount { get; set; }
        public List<FileDetails> FileExclusions { get { return _fileExclusions; } }
        private List<string> FilterList { get; set; }
        public string Runner { get; set; }
        public string EnvironmentInfo { get; set; }

        #endregion

        #region Private Methods

        private bool IsExclusion(string file)
        {
            var fileInfo = new FileInfo(file);
            if (!(fileInfo.Length > 0 && fileInfo.Length <= MaxSize))
            {
                FileDetails fileDetails = CommonUtility.CreateFileDetails(file,
                                                                          Runner,
                                                                          Logger,
                                                                          EnvironmentInfo);
                if (null != fileDetails)
                    FileExclusions.Add(fileDetails);
                return true;
            }
            return false;
        }

        override protected IEnumerable<string> GetAllFilesInDirectory( string directoryPath,
                                                                       Delimon.Win32.IO.SearchOption searchOption)
        {
            IEnumerable<string> files = null;
            IEnumerable<string> subdirectories = null;

            GetEntries(directoryPath, ref files, ref subdirectories);

            if (files != null)
            {
                foreach (string file in files)
                {
                    _currentFiles.Clear();
                    if (FilterList.Contains(file) || IsExclusion(file)) continue;
                    UpdateResults(file);
                    if (_yieldCount >= ReturnCount) break;
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
                            _currentFiles.Clear();
                            if (FilterList.Contains(file) || IsExclusion(file)) continue;
                            UpdateResults(file);
                            if (_yieldCount >= ReturnCount) break;
                            yield return file;
                        }
                    }
                    else if (null != Logger)
                        Logger.Warn(string.Format("{0} --- JUNCTION detected, skipped", subdirectory));
                }
            }
        }

        private void UpdateResults(string file)
        {
            _currentFiles.Add(file);
            _yieldCount++;
            RaiseBatchEvent();
        }

        #endregion
    }
}
