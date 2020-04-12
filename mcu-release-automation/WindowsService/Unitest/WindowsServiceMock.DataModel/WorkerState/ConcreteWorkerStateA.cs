
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerStateA : MockWorkerState
    {
        #region Constructor

        public ConcreteWorkerStateA(ILog logger)
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
