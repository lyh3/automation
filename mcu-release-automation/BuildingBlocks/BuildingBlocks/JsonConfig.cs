using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Automation.Base.BuildingBlocks
{
    public enum TransactionStatus
    {
        Idle,
        Progress,
        Completed,
        Failed,
        Aborted
    }
    [Serializable]
    public class JsonConfig
    {
        private TransactionStatus _tran = TransactionStatus.Idle;
        public string McuDropBox { get; set; }
        public string WorkSpace { get; set; }
        public int timeout { get; set; }
        public string LogName { get; set; }
        public bool DryRun { get; set; }
        public bool OneTimeOnly { get; set; }
        public string TransStatus
        {
            get { return _tran.ToString(); }
            set
            {
                _tran = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), value);
            }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public abstract class StateGroup
    {
        public string key { get; set; }
        public bool skip { get; set; }
        [JsonIgnore]
        public Tuple<string, State>[] ResultsTuple
        {
            get
            {
                var resultsTuples = new List<Tuple<string, State>>();
                foreach (var state in _getStates())
                {
                    foreach (var propertyInfo in state.GetType().GetProperties())
                    {
                        var val = propertyInfo.GetValue(state);
                        if (null != val && val is State)
                        {
                            resultsTuples.Add(new Tuple<string, State>(propertyInfo.Name, val as State));
                        }
                    }
                }
                return resultsTuples.ToArray();
            }
        }
        protected abstract dynamic[] _getStates();
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class StateData
    {
        public string path { get; set; }
        public string command { get; set; }
        public string script { get; set; }
        public string repoUrl { get; set; }
        public List<string> parameters { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class State
    {
        public bool skip { get; set; }
        public string status { get; set; }
        public string error { get; set; }
        public StateData stateData { get; set; }
        public bool waitkeystroke { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}
