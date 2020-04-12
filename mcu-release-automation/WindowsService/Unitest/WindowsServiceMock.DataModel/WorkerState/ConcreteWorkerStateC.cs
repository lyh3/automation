
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerStateC : MockWorkerState
    {
        #region Constructor

        public ConcreteWorkerStateC(ILog logger)
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
