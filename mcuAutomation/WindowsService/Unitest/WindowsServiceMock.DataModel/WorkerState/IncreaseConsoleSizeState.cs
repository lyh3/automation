using System;
using System.Drawing;

using log4net;

namespace McAfee.Service.DataModel
{
    public class IncreaseConsoleSizeState : MockWorkerState
    {
        #region Constructor

        public IncreaseConsoleSizeState(ILog logger)
            : base(logger)
        {
        }

        #endregion

        protected override void DoWork()
        {
            Success = false;
            if (_thisConsole == IntPtr.Zero)
                return;

            try
            {
                // Get the current window dimensions. 
                RECT rct;
                if (!GetWindowRect(_thisConsole, out rct))
                    return;

                var rect = new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top);
                _logger.Info(string.Format(@"---IncreaseConsoleSizeState, current console width = <{0}>, height = <{1}>", rect.Width, rect.Height));

                var b = MoveWindow(_thisConsole, rect.X, rect.Y, rect.Width, rect.Height + 30, true);
                Success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format(@"---Exception caught at IncreaseConsoleSizeState, error was : {0}", ex.Message));
            }
        }
    }
}
