using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using log4net;
using Automation.WorkerThreadModel;

namespace WindowService.DataModel
{
    public class MonitorSetupState : HsdReleaseMonitorState
    {
        private bool _serviceStarted = false;
        public MonitorSetupState(WorkerThread parent, ILog logger) : base(parent, logger)
        {
        }
        protected override void DoWork()
        {
            try
            {
                var hsdMcuReleaseConfig = (ParentThread as McuReleaseMonitorWorkerThread).HsdMcuReleaseJsonConfig;  
                var logfileWatcher = new FileSystemWatcher();
                var parentTheread = this.ParentThread as McuReleaseMonitorWorkerThread;
                var workingdirectory = parentTheread.MonitorFolder;
                logfileWatcher.Path = workingdirectory;
                logfileWatcher.NotifyFilter = NotifyFilters.LastAccess
                                                | NotifyFilters.LastWrite;
                logfileWatcher.Filter = "*.log";
                logfileWatcher.Changed += onLogFileChanged;
                logfileWatcher.EnableRaisingEvents = true;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    Thread.Sleep(1000);
                } while (stopwatch.Elapsed.Minutes < hsdMcuReleaseConfig.Schedule.AlertReportGracePeriodInMin);

                if (!_serviceStarted)
                {
                    parentTheread.TaskQueue.Enqueue(new SendAlertNotificationtate(ParentThread, _logger));
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(@"Exception caught at MonitorSetupState, error = {0}", ex.Message);
                _success = false;
            }
        }

        private void onLogFileChanged(object sender, FileSystemEventArgs e)
        {
            var parentTheread = this.ParentThread as McuReleaseMonitorWorkerThread;
            if (parentTheread != null)
            {
                if (Path.GetFileName(e.Name).EndsWith(parentTheread.MonitorFile))
                {
                    _serviceStarted = true;
                }
            }
        }
    }
}
