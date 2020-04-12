
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerStateB : MockWorkerState
    {
        #region Constructor

        public ConcreteWorkerStateB(ILog logger)
            : base(logger)
        {
        }

        #endregion

        protected override void DoWork()
        {
            _logger.Info(string.Format(@"--- <{0}> DoWork", this.GetType().Name));
        }
    }
}
