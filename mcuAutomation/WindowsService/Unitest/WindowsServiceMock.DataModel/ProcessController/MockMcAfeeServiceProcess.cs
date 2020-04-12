using System.Collections.Generic;
using System.Linq;

using log4net;

namespace McAfee.Service.DataModel
{
    abstract public class MockMcAfeeServiceProcess : McAfeeServiceProcess
    {
        #region Declarations

        protected List<MockWorkerThread> _workerList = new List<MockWorkerThread>();

        #endregion

        #region Constructors

        public MockMcAfeeServiceProcess() { }
        public MockMcAfeeServiceProcess(ILog logger = null)
            : base(logger)
        {
            InitializeWorkerProcess();
        }

        #endregion

        #region Public Methods

        override public void Start()
        {
            _workerList.ForEach(workerthread => workerthread.Start());
        }

        override public void Stop()
        {
            _workerList.ForEach(workerthread => workerthread.Stop());
        }

        #endregion
    }
}
