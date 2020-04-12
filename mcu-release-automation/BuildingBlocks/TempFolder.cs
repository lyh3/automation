// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace McAfeeLabs.Engineering.Automation.Base
{
    /// <summary>
    /// Class to encapsulate a temp folder that only needs to exist for a short time. On
    /// exit this class will recursively remove the temp folder and it's contents
    /// </summary>
    public class TempFolder : IDisposable
    {
        /// <summary>
        /// The temporary folder name
        /// </summary>
        public String FolderName { get; private set; }

        /// <summary>
        /// Construct a new tempfolder object and create the folder. 
        /// </summary>
        /// <param name="extraPath">An extra path parts that needs to be added to the base temp folder</param>
        public TempFolder(String extraPath = null)
        {
            FolderName = Path.GetTempPath() + Guid.NewGuid().ToString();
            if (extraPath != null)
                FolderName = Path.Combine(FolderName, extraPath);

            Directory.CreateDirectory(FolderName);
        }

        /// <summary>
        /// Combine the temp folder with one or more other folders
        /// </summary>
        /// <param name="pathParts"></param>
        /// <returns></returns>
        public String Combine(params String[] pathParts)
        {
            if (pathParts == null || pathParts.Length == 0)
                return null;

            String retval = FolderName;
            foreach (String s in pathParts)
                retval = Path.Combine(retval, s);

            return retval;
        }

        /// <summary>
        /// Create a sub folder off the temp folder.
        /// </summary>
        /// <param name="path"></param>
        public void CreateSubFolder(params String[] pathParts)
        {
            String f = Combine(pathParts);
            if (!Directory.Exists(f))
                Directory.CreateDirectory(f);
        }

        /// <summary>
        /// Clean up and remove the folder if still exists
        /// </summary>
        public void Dispose()
        {
            if (Directory.Exists(FolderName))
                Directory.Delete(FolderName, true);
        }
    }
}
