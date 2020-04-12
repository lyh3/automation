
using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteWorkerStateD : MockWorkerState
    {
        #region Constructor

        public ConcreteWorkerStateD(ILog logger)
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
