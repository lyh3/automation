
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerThreadD : MockWorkerThread
    {
        #region Constructor

        public ConcreteWorkerThreadD(ILog logger) 
            : base(logger)
        {
        }

        #endregion

        protected override WorkerState InitialWork()
        {
            return new ConcreteWorkerStateC(_logger);
        }

        public override WorkerState StateFactory(WorkerState args = null)
        {
            WorkerState state = null;
            var currentState = args as MockWorkerState;

            if (null == args)
                state = InitialWork();
            else if (args is ConcreteWorkerStateC && currentState.Success)
                state = new ConcreteWorkerStateD(_logger);
            return state;
        }
    }
}
