using System;
using System.Collections.Generic;
using System.IO;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class FolderInspecterCopyReceiver : FolderInspectorReceiver
    {
        private string _receiveLocation;

        public FolderInspecterCopyReceiver( IEnumerable<string> sourceFolders, 
                                            int batchSize, 
                                            string receiveLocation,
                                            bool recursive = true)
            : base(sourceFolders, batchSize, int.MaxValue, recursive)
        {
            if (!Directory.Exists(receiveLocation)) Directory.CreateDirectory(receiveLocation);
            _receiveLocation = receiveLocation;
        }

        public void Pause()
        {
            if (null != _folderInspector)
                _folderInspector.Pause = true;
        }

        public void Resume()
        {
            if (null != _folderInspector)
                _folderInspector.Pause = false;
        }

        override protected void OnFileInspectBatchResults(object sender, EventArgs e)
        {
            _folderInspector.CurrentFiles.ForEach(sourceFile =>
            {
                var fileName = Path.GetFileName(sourceFile);
                var targetFilePath = Path.Combine(_receiveLocation, fileName);
                if(!File.Exists(targetFilePath))
                    File.Copy(sourceFile, targetFilePath);
            });
        }

        override protected void InitializeReceiver( IEnumerable<string> sourceFolders, 
                                                    int batchSize, 
                                                    bool recursive,
                                                    int returnCount)
        {
            _folderInspector = new FolderInspector(sourceFolders, recursive) { BatchSize = batchSize };
            _folderInspector.BatchResults += OnFileInspectBatchResults;
        }
    }
}
