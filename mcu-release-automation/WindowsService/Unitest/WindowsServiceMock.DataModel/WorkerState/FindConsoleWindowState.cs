using System;
using log4net;

namespace McAfee.Service.DataModel
{
    public class FindConsoleWindowState : MockWorkerState
    {
        #region Constructor

        public FindConsoleWindowState(ILog logger)
            : base(logger)
        {
        }

        #endregion

        protected override void DoWork()
        {
            _logger.Info(string.Format(@"--- <{0}> DoWork", this.GetType().Name));
            _thisConsole = GetConsoleWindow();
            if (_thisConsole != IntPtr.Zero)
                Success = true;
        }
    }
}
