using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class HsdMcuAutoReleaseDataModel : JsonConfig
    {
        public ProcessingGroup Processing { get; set; }
        public ReleaseSchedule Schedule { get; set; }
        public string HsdEsRestUrl { get; set; }
        public long QueryId { get; set; }
        public int GitKeeperScore { get; set; }
        public List<List<string>> HsdArticle { get; set; }
    }
    [Serializable]
    public class ReleaseSchedule
    {
        private ReleaseFrequency _releaseFrequecy = ReleaseFrequency.Hourly;
        private DateTime? _lastRun = null;
        public int AlertReportGracePeriodInMin { get; set; }
        public string Frequency
        {
            get { return _releaseFrequecy.ToString(); }
            set
            {
                _releaseFrequecy = (ReleaseFrequency)Enum.Parse(typeof(ReleaseFrequency), value);
            }
        }
        public int Every { get; set; }
        public string LastRun
        {
            get
            {
                return _lastRun.HasValue ? _lastRun.Value.ToString(EmailService.DateDispFormatMMss) : "2020-01-01 00:00:00";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime dt;
                    if (DateTime.TryParse(value, out dt))
                    {
                        _lastRun = dt;
                    }
                }
            }
        }
        public bool AllowToSendAlert { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    public class SendEmailState : State
    {
        new public SendNotificationStateData stateData { get; set; }
    }
    public class SendNotificationStateData : StateData
    {
        private List<SMTPEmailConfig> _parameters = new List<SMTPEmailConfig>();
        new public SMTPEmailConfig[] parameters
        {
            get { return _parameters.ToArray(); }
            set { _parameters.Clear(); _parameters.AddRange(value); }
        }
    }
    public class download_mcu_state : State { }
    public class create_request_state : State { }
    public class update_hsd_submit_state : State { }
    public class send_notification_state : SendEmailState { }
    public class HsdProcessingState
    {
        public download_mcu_state download_mcu { get; set; }
        public create_request_state create_request { get; set; }
        public update_hsd_submit_state update_hsd_submit { get; set; }
        public send_notification_state send_notification { get; set; }

        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    public class ProcessingGroup : StateGroup
    {
        private List<HsdProcessingState> _stateList = new List<HsdProcessingState>();
        public HsdProcessingState[] States
        {
            get { return _stateList.ToArray(); }
            set { _stateList.Clear(); _stateList.AddRange((HsdProcessingState[])value); }
        }
        protected override dynamic[] _getStates() { return States; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    public class SMTPEmailConfig
    {
        private readonly List<string> _to = new List<string>();
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string[] To
        {
            get { return _to.ToArray(); }
            set { _to.Clear(); _to.AddRange(value); }
        }
        public string BodyFormat { get; set; }
        public string Password { get; set; }
    }
}
public enum ReleaseFrequency
{
    Monthly,
    Daily,
    Hourly
}
