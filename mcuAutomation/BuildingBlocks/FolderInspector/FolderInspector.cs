using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class FolderInspector : FolderInspectorBase
    {
        #region Constructor

        public FolderInspector(IEnumerable<string> folders, bool recursive = true)
            : base(folders, recursive)
        {
        }

        public FolderInspector(IEnumerable<string> folders)
            : base(folders)
        {
        }

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
                    if (_isComplete) break;
                    while (Pause) { Application.DoEvents(); }
                    
                    if (!f.Contains(file))
                    {
                        f.Add(file.LongPathFormat());
                    }
                    CurrentFiles = f;
                    yield return file;
                }
            }

            if (subdirectories != null && searchOption == Delimon.Win32.IO.SearchOption.AllDirectories)
            {
                foreach (string subdirectory in subdirectories)
                {
                    if (_isComplete) break;
                    var valid = false;
                    try
                    {
                        valid = (Delimon.Win32.IO.File.GetAttributes(subdirectory) & Delimon.Win32.IO.FileAttributes.ReparsePoint) != Delimon.Win32.IO.FileAttributes.ReparsePoint;
                    }
                    catch(Exception ex)
                    {
                        if (null != Logger)
                            Logger.Error(string.Format("{0} --- Exception at GetAllFilesInDirectory, error was : {0}", ex.Message));
                    }
                    if (valid)
                    {
                        foreach (string file in GetAllFilesInDirectory(subdirectory, searchOption))
                        {
                            if (_isComplete) break;
                            while (Pause) { Application.DoEvents(); }
                            if (!CurrentFiles.Contains(file))
                            {
                                CurrentFiles.Add(file.LongPathFormat());
                            }
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
