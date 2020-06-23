using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Automation.WorkerThreadModel;
using WindowService.DataModel;

namespace McuReleaseSubmitService.DataModel
{
    public class McuReleaseAutomationMonitorProcess : WindowServiceProcess
    {
        private List<McuReleaseMonitorWorkerThread> _workthreadList = new List<McuReleaseMonitorWorkerThread>();
        public McuReleaseAutomationMonitorProcess(Dictionary<string, Tuple<string, string, string>> dic, ILog logger)
        {
            _logger = logger;
            var itr = dic.GetEnumerator();
            while (itr.MoveNext())
            {
                switch (itr.Current.Value.Item3)
                {
                    case "McuReleaseSubmitApp":
                        _workthreadList.Add(new McuReleaseProcessMonitorWorkerThread(itr.Current.Key, _logger)
                        {
                            ServiceName = itr.Current.Value.Item3
                        });
                        break;
                    default:
                        _workthreadList.Add(new McuReleaseFileMonitorWorkerThread(itr.Current.Key, _logger)
                        {
                            MonitorFolder = itr.Current.Value.Item1,
                            MonitorFile = itr.Current.Value.Item2,
                            ServiceName = itr.Current.Value.Item3
                        });
                        break;
                }
            }
        }
        override protected void InitializeWorkerProcess()
        {
        }
        public override void Start()
        {
            _workthreadList.ForEach(x => x.Start());
        }
    }
}

