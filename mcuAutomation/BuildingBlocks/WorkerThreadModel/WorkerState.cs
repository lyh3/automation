using System;

using log4net;

namespace McAfeeLabs.Engineering.Automation.WorkerThreadModel
{
    abstract public class WorkerState : ICommand
    {
        #region Declarations

        protected ILog _logger;
        protected bool _success = false;
        private event EventHandler _eventStateComplete;

        #endregion

        #region Constructor

        public WorkerState(ILog logger)
        {
            _logger = logger;
        }

        #endregion

        #region Properties

        public event EventHandler StateComplete
        {
            add { _eventStateComplete = (EventHandler)Delegate.Combine(_eventStateComplete, value); }
            remove { _eventStateComplete = (EventHandler)Delegate.Remove(_eventStateComplete, value); }
        }

        public bool Success 
        {
            get { return _success; }
            set { _success = value; }
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            try
            {
                DoWork();
            }
            finally
            {
                if (null != _eventStateComplete)
                    _eventStateComplete(this, null);
            }
        }

        protected abstract void DoWork();

        #endregion
    }
}
