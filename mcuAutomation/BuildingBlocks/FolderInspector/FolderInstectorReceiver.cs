using System;
using System.Collections.Generic;

namespace McAfeeLabs.Engineering.Automation.Base
{
    abstract public class FolderInspectorReceiver : IInspector
    {
        #region Declaration

        protected FolderInspectorBase _folderInspector;
        protected List<string> _receivedFiles = new List<string>();
        protected bool _canDispose = true;
        protected int _maxSize = int.MaxValue;
        protected List<string> _filterList = new List<string>();
        protected event EventHandler _eventBatchResultsForward;

        #endregion

        #region Constructor

        public FolderInspectorReceiver( IEnumerable<string> sourceFolders,
                                        int batchSize = 10,        
                                        int returnCount = int.MaxValue, 
                                        bool recursive = true,
                                        int maxSize = int.MaxValue,
                                        IEnumerable<string> filterList = null)
        {
            _maxSize = maxSize;
            if (null != filterList)
                _filterList.AddRange(filterList);
            InitializeReceiver(sourceFolders, batchSize, recursive, returnCount);
        }

        #endregion

        #region Public Methods

        public IEnumerable<string> Inspect()
        {
            return _folderInspector.Inspect();
        }

        virtual public void Dispose()
        {
            if (null != _folderInspector)
                _folderInspector.Dispose();
        }

        #endregion

        #region Properties

        public event EventHandler BatchResultsForward
        {
            add { _eventBatchResultsForward = (EventHandler)Delegate.Combine(_eventBatchResultsForward, value); }
            remove { _eventBatchResultsForward = (EventHandler)Delegate.Remove(_eventBatchResultsForward, value); }
        }
        virtual public bool CanDispose
        {
            get { return _canDispose; }
            set{_canDispose = value;}
        }
        public List<string> ReceivedFiles { get { return _receivedFiles; } }
        public int BatchSize
        {
            get { return _folderInspector.BatchSize; }
            set { _folderInspector.BatchSize = value; }
        }
        public FolderInspectorBase Inspector 
        { 
            get { return _folderInspector; }
            set { _folderInspector = value; }
        }
        public bool IsComplete { get { return _folderInspector.IsComplete; } }

        #endregion

        protected void RaiseForwardEvent()
        {
            if (null != _eventBatchResultsForward)
                _eventBatchResultsForward(this, EventArgs.Empty);
        }

        abstract protected void InitializeReceiver(IEnumerable<string> sourceFolders, int batchSize, bool recursive, int returnCount);
        abstract protected void OnFileInspectBatchResults(object sender, EventArgs e);
    }
}
