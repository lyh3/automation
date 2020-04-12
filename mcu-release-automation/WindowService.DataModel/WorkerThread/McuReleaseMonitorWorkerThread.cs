using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using log4net;
using Automation.WorkerThreadModel;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    abstract public class McuReleaseMonitorWorkerThread : WorkerThread
    {
        protected string _jsonConfigPath = null;
        protected HsdMcuAutoReleaseDataModel _hsdMcuReleaseJsonConfig = null;
        protected Queue<WorkerState> _queue = null;
        public McuReleaseMonitorWorkerThread(string jsonConfigPath, ILog logger)
            : base(logger)
        {
            _jsonConfigPath = jsonConfigPath;
        }
        public HsdMcuAutoReleaseDataModel HsdMcuReleaseJsonConfig
        {
            get { return _hsdMcuReleaseJsonConfig; }
            set { _hsdMcuReleaseJsonConfig = value; }
        }
        public Queue<WorkerState> TaskQueue
        {
            get { return _queue; }
        }
        public string MonitorFolder { get; set; }
        public string MonitorFile { get; set; }
        public string ServiceName { get; set; }
        public override WorkerState StateFactory(WorkerState args = null)
        {
            WorkerState state = null;
            if (null == args)
            {
                state = InitialWork();
            }
            else
            {
                if (!(args as HsdReleaseMonitorState).Success)
                {
                    _queue.Clear();
                }
                else if (_queue.Count > 0)
                {
                    state = _queue.Dequeue();
                }
            }
            if (null == state && _hsdMcuReleaseJsonConfig.OneTimeOnly)
            {
                this.Stop();
            }
            return state;
        }
        protected bool _IsOnSchedure()
        {
            var isOnScheduled = false;
            var timeNow = DateTime.Now;
            DateTime lastrun;
            DateTime.TryParse(_hsdMcuReleaseJsonConfig.Schedule.LastRun, out lastrun);
            var timeSpan = timeNow - lastrun;
            var releaseFrequecy = (ReleaseFrequency)Enum.Parse(typeof(ReleaseFrequency), _hsdMcuReleaseJsonConfig.Schedule.Frequency);

            var n = _hsdMcuReleaseJsonConfig.Schedule.Every;
            switch (releaseFrequecy)
            {
                case ReleaseFrequency.Monthly:
                    isOnScheduled = timeSpan.Days >= 30 * n;
                    break;
                case ReleaseFrequency.Daily:
                    isOnScheduled = timeSpan.Days >= n;
                    break;
                case ReleaseFrequency.Hourly:
                default:
                    isOnScheduled = timeSpan.Hours >= n;
                    break;
            }
            return isOnScheduled;
        }
    }
    public class McuReleaseFileMonitorWorkerThread : McuReleaseMonitorWorkerThread
    {
        public McuReleaseFileMonitorWorkerThread(string jsonConfigPath, ILog logger)
            : base(jsonConfigPath, logger)
        {
        }
        protected override WorkerState InitialWork()
        {
            WorkerState state = null;
            try
            {
                using (StreamReader file = File.OpenText(_jsonConfigPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    _hsdMcuReleaseJsonConfig = (HsdMcuAutoReleaseDataModel)serializer.Deserialize(file, typeof(HsdMcuAutoReleaseDataModel));
                }
                if (null != _hsdMcuReleaseJsonConfig
                    && !_hsdMcuReleaseJsonConfig.Processing.skip
                    && _hsdMcuReleaseJsonConfig.Schedule.AllowToSendAlert)
                {
                    if (_IsOnSchedure())
                    {
                        _queue = new Queue<WorkerState>();
                        _queue.Enqueue(new MonitorSetupState(this, _logger));
                        state = _queue.Dequeue();
                        _hsdMcuReleaseJsonConfig.Schedule.LastRun = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss");
                        File.WriteAllText(_jsonConfigPath, _hsdMcuReleaseJsonConfig.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(@"Exception caught at McuReleaseFileMonitorWorkerThread InitialWoek, error = {0}", ex.Message);
            }
            return state;
        }
    }
    public class McuReleaseProcessMonitorWorkerThread : McuReleaseMonitorWorkerThread
    {
        public McuReleaseProcessMonitorWorkerThread(string jsonConfigPath, ILog logger)
            : base(jsonConfigPath, logger)
        {
        }
        protected override WorkerState InitialWork()
        {
            WorkerState state = null;
            try
            {
                using (StreamReader file = File.OpenText(_jsonConfigPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    _hsdMcuReleaseJsonConfig = (HsdMcuAutoReleaseDataModel)serializer.Deserialize(file, typeof(HsdMcuAutoReleaseDataModel));
                }
                if (null != _hsdMcuReleaseJsonConfig
                    && _hsdMcuReleaseJsonConfig.Schedule.AllowToSendAlert)
                {
                    if (_IsOnSchedure())
                    {
                        if (!string.IsNullOrEmpty(ServiceName) && !ServiceName.FindProcess())
                        {
                            _queue = new Queue<WorkerState>();
                            _queue.Enqueue(new SendAlertNotificationtate(this, _logger));
                            state = _queue.Dequeue();
                        }
                        _hsdMcuReleaseJsonConfig.Schedule.LastRun = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss");
                        File.WriteAllText(_jsonConfigPath, _hsdMcuReleaseJsonConfig.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(@"Exception caught at McuReleaseProcessMonitorWorkerThread InitialWoek, error = {0}", ex.Message);
            }
            return state;
        }
    }
}
