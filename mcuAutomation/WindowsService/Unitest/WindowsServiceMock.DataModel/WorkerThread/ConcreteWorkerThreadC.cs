
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerThreadC : MockWorkerThread
    {
        #region Constructor

        public ConcreteWorkerThreadC(ILog logger) 
            : base(logger)
        {
        }

        #endregion

        protected override WorkerState InitialWork()
        {
            return new ConcreteWorkerStateA(_logger);
        }

        public override WorkerState StateFactory(WorkerState args = null)
        {
            WorkerState state = null;
            var currentState = args as MockWorkerState;

            if (null == args)
                state = InitialWork();
            else if (args is ConcreteWorkerStateA && currentState.Success)
                state = new ConcreteWorkerStateB(_logger);
            return state;
        }
    }
}
