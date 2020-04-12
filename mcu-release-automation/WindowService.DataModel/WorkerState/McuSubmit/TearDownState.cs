using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using log4net;
using Automation.WorkerThreadModel;

namespace WindowService.DataModel
{
    public class TearDownState : McuSubmitState
    {
        public TearDownState(WorkerThread parent, ILog logger) : base(parent, logger)
        {
        }

        protected override void DoWork()
        {
            McuReleaseSubmitWorkerThread parent = (McuReleaseSubmitWorkerThread)ParentThread;
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    if (File.Exists(parent.Request))
                    {
                        File.Delete(parent.Request);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat(@"Exception caught at TearDownState, error = {0}", ex.Message);
                    _success = false;
                }
            }
        }
    }
}
