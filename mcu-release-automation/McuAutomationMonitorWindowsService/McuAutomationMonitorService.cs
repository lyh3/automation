using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using log4net;
using Windows.Service;
using Automation.WorkerThreadModel;
using McuReleaseSubmitService.DataModel;
using WindowService.DataModel;
using System.IO;

namespace McuAutomationMonitorWindowsService
{
    public partial class McuAutomationMonitorService : WindowServiceThreadModel
    {
        public McuAutomationMonitorService(string serviceName, string serviceDescription)
            : base(serviceName, serviceDescription)
        {
        }

        public McuAutomationMonitorService(string serviceName, string serviceDescription, ILog logger)
            : this(serviceName, serviceDescription)
        {
            _logger = logger;
        }

        override protected void InitializeWorkerThreads()
        {
            var monitorDictionary = new Dictionary<string, Tuple<string, string, string>>();
            var monitorFolder = ConfigurationManager.AppSettings["MonitorFolder"];
            monitorDictionary.Add(ConfigurationManager.AppSettings["HsdEsMicrocodeReleaseConfig"],
                                  new Tuple<string, string, string>(monitorFolder,
                                                                    ConfigurationManager.AppSettings["HsdEsMicrocodeReleaseMonitorFile"],
                                                                    @"HsdEsMicrocodeReleaseAutomation"));
            monitorDictionary.Add(ConfigurationManager.AppSettings["ArticleIdMapConfig"],
                                  new Tuple<string, string, string>(monitorFolder,
                                                                    ConfigurationManager.AppSettings["ArticleIdMapMonitorFile"],
                                                                    @"HsdMcuMap"));
            monitorDictionary.Add(ConfigurationManager.AppSettings["MiddlewareMoitorConfig"],
                                  new Tuple<string, string, string>(string.Empty,
                                                                    string.Empty,
                                                                    @"McuReleaseSubmitApp"));
            _serviceThreadProcess.AddRange(new[] { (IWindowServiceProcess)new McuReleaseAutomationMonitorProcess(monitorDictionary, _logger) });
            StartThreadProcess();
        }
    }
}
