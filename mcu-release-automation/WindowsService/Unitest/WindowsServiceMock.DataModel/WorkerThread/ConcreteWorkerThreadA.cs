
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerThreadA : MockWorkerThread
    {
        #region Constructor

        public ConcreteWorkerThreadA(ILog logger) 
            : base(logger)
        {
        }

        #endregion

        protected override WorkerState InitialWork()
        {
            return new ConcreteWorkerStateA(_logger);//FindConsoleWindowState(_logger);// 
        }

        public override WorkerState StateFactory(WorkerState args = null)
        {
            WorkerState state = null;
            var currentState = args as MockWorkerState;

            if (null == args)
                state = InitialWork();
            else if (args is ConcreteWorkerStateA && currentState.Success)
            //else if (args is FindConsoleWindowState && currentState.Success)
                state = new ConcreteWorkerStateB(_logger);// IncreaseConsoleSizeState(_logger);//

            return state;
        }
    }
}
