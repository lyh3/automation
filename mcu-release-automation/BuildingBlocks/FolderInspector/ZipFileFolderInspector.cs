using System.Collections.Generic;

using Ionic.Zip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class ZipFileFolderInspector : FolderInspectorBase
    {
        #region Declarations

        private int _deepLevel = 0;

        #endregion

        #region Constructor

        public ZipFileFolderInspector(IEnumerable<string> folders, bool recursive = true)
            : base(folders, recursive)
        {
        }

        public ZipFileFolderInspector(IEnumerable<string> folders)
            : base(folders)
        {
        }

        #endregion

        #region Properties

        public int DeepLevel { get { return _deepLevel; } }

        #endregion

        #region Private Methods

        override protected IEnumerable<string> GetAllFilesInDirectory( string directoryPath, 
                                                                       Delimon.Win32.IO.SearchOption searchOption)
        {
            IEnumerable<string> files = null;
            IEnumerable<string> subdirectories = null;

            GetEntries(directoryPath, ref files, ref subdirectories);

            if (files != null)
            {
                var f = _currentFiles;
                foreach (string file in files)
                {
                    if (!ZipFile.IsZipFile(file)) continue;
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
                            if (!ZipFile.IsZipFile(file)) continue;
                            CurrentFiles.Add(file.LongPathFormat());
                            _deepLevel++;
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
