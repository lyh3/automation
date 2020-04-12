using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Automation.WorkerThreadModel;
using WindowService.DataModel;

namespace McuReleaseSubmitService.DataModel
{
    public class McuReleaseSubmitProcess : WindowServiceProcess
    {
        private McuReleaseSubmitWorkerThread _mcuReleaseThread = null;
        public McuReleaseSubmitProcess(ILog logger)
        {
            _logger = logger;
            _mcuReleaseThread = new McuReleaseSubmitWorkerThread(_logger);
        }
        override protected void InitializeWorkerProcess()
        {
           
        }
        public override void Start()
        {
            _mcuReleaseThread.Start();
        }
    }
}
                                                                                               
