using System;
using System.Collections.Generic;
using System.Linq;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class FilteredFolderInspectorReceiver : FolderInspectorReceiver
    {
        #region Constructor

        public FilteredFolderInspectorReceiver( IEnumerable<string> sourceFolders,
                                                int batchSize = 1,
                                                bool recursive = true,
                                                IEnumerable<string> filterList = null,
                                                int returnCount = int.MaxValue,
                                                int maxSize = int.MaxValue)
            : base(sourceFolders, batchSize, returnCount, recursive, maxSize, filterList)
        {
        }

        #endregion

        public List<FileDetails> FileExclusionsYield { get { return (_folderInspector as FilteredFolderInspector).FileExclusions; } }


        override protected void InitializeReceiver( IEnumerable<string> sourceFolders, 
                                                    int batchSize, 
                                                    bool recursive,
                                                    int returnCount)
        {
            _folderInspector = new FilteredFolderInspector( sourceFolders,
                                                            batchSize,
                                                            recursive,
                                                            _filterList,
                                                            returnCount, 
                                                            _maxSize);
            _folderInspector.BatchResults += OnFileInspectBatchResults;
        }

        override protected void OnFileInspectBatchResults(object sender, EventArgs e)
        {
            _folderInspector.CurrentFiles.ForEach(x =>
            {
                if (!_receivedFiles.Contains(x))
                    _receivedFiles.Add(x);
            });

            if (_folderInspector.IsComplete)
            {
                _folderInspector.BatchResults -= OnFileInspectBatchResults;
                RaiseForwardEvent();
            }
        }

    }
}
