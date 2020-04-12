
using log4net;

namespace McAfee.Service.DataModel
{
    abstract public class MockWorkerThread : WorkerThread
    {
        #region Constructor

        public MockWorkerThread(ILog logger) 
            : base(logger)
        {
        }

        #endregion
    }
}
