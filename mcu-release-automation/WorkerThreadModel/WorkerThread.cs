using System;
using System.Threading;

using log4net;

namespace Automation.WorkerThreadModel
{
    abstract public class WorkerThread : IWorkerThread
    {
        #region Declarations

        protected const int DefaultRecurringInterval = 3 * 1000;
        protected static ILog _logger;
        protected Thread _workerThread;
        protected bool _isRunning;
        protected int _recurringInterval = DefaultRecurringInterval;

        #endregion

        #region Constructor

        public WorkerThread( ILog logger )
        {
            _logger = logger;
        }

        #endregion

        #region Properties 
  
        public bool IsRunning 
        { 
            get 
            {
                if(null != _workerThread)
                    return _workerThread.ThreadState == ThreadState.Running || _workerThread.ThreadState == ThreadState.WaitSleepJoin;
                return _isRunning;
            } 
        }

        public string EnvironmentInfo { get { return string.Format(@"[Host:{0}]", Environment.MachineName); } }
        protected int RecurringInterval
        {
            get { return _recurringInterval; }
            set { _recurringInterval = value; }
        }

        #endregion

        public void Start()
        {
            try
            {
                _workerThread = new Thread(() => WorkerProcess());
                _workerThread.Start();
            }
            catch (Exception ex)
            {
                if(null != _logger)
                    _logger.Warn(string.Format(@"--- Exception caught at Start worker, error was:{0}", ex.Message));
            }
        }

        public void Stop()
        {
            if (null != _workerThread
                && _workerThread.ThreadState == ThreadState.Running)
            {
                _workerThread.Abort();//Join();
                Thread.Sleep(100);
            }
            _isRunning = false;
        }

        public void ExecuteState(WorkerState state)
        {
            if (null == state) 
                return;
            try
            {
                state.StateComplete += OnStateComplete;
                state.Execute();
            }
            catch (Exception ex)
            {
                var errormsg = string.Format(@"--- {3} Exception caught at ExecuteState, the error was:{2}{0}, stack trace:{2}{1}",
                                             null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                             ex.StackTrace,
                                             Environment.NewLine,
                                             EnvironmentInfo);
                if (null != _logger)
                    _logger.Error(errormsg);
            }
            finally
            {
                if (null != state)
                    state.StateComplete -= OnStateComplete;
            }
        }

        void OnStateComplete(object sender, EventArgs e)
        {
            WorkerState state = sender as WorkerState;
            if (null != state)
                state.StateComplete -= OnStateComplete;

            state = StateFactory(state);

            if (null != state)
                ExecuteState(state);
        }

        protected void WorkerProcess()
        {
            while (IsRunning)
            {
                var state = StateFactory();
                if (null != state)
                    ExecuteState(state);

                Thread.Sleep(_recurringInterval);
            }
        }

        abstract public WorkerState StateFactory(WorkerState args = null);
        abstract protected WorkerState InitialWork();
    }
}
