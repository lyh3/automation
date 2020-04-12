using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

using log4net;
using Automation.WorkerThreadModel;

namespace IntelDCGSpsWebService.Models
{
    public class BiosRelease_GenerateReleaseNote_State : BiosReleaseState
    {
        public BiosRelease_GenerateReleaseNote_State(WorkerThread parent, ILog logger) : base(parent, logger) { }

        protected override void DoWork()
        {
            try
            {
                NotificationMessages = string.Format("{0} in progressing ...", StateTag);
                Thread.Sleep(5000);
            }
            catch(Exception ex)
            {
                var msg = string.Format("Error occurs at BiosRelease_GenerateReleaseNote_State, error = {0}", ex.Message);
                NotificationMessages = msg;
                _logger.Error(msg);
                _success = false;
            }
        }
    }
}