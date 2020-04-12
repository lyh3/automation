using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class WorkflowConfigModel
    {
        private TransactionStatus _tran = TransactionStatus.Idle;
        public WorkflowConfigModel(){}
        public WorkflowConfigModel(WorkflowConfigModel model):this()
        {
            Setup = model.Setup;
            Processing = model.Processing;
            Publish = model.Publish;
            McuDropBox = model.McuDropBox;
            WorkSpace = model.WorkSpace;
            timeout = model.timeout;
            LogName = model.LogName;
            TestingRepoEx = model.TestingRepoEx;
            DryRun = model.DryRun;
            OneTimeOnly = model.OneTimeOnly;
            RebaseSilent = model.RebaseSilent;
            TransStatus = model.TransStatus;
            TransError = model.TransError;
        }
        public Setup Setup { get; set; }
        public Processing Processing { get; set; }
        public Publish Publish { get; set; }
        public string McuDropBox { get; set; }
        public string WorkSpace { get; set; }
        public string timeout { get; set; }
        public string LogName { get; set; }
        public string TestingRepoEx { get; set; }
        public bool DryRun { get; set; }
        public bool OneTimeOnly { get; set; }
        public bool RebaseSilent { get; set; }
        public string TransError { get; set; }
        public string SessionId { get; set; }
        public string SessionLog { get; set; }
        public string TransStatus
        {
            get { return _tran.ToString(); }
            set
            {
                _tran = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), value);
            }
        }
        public string LogContents
        {
            get
            {
                var regex = new Regex(@"(_([a-z0-9]{24}))");
                if (!regex.Match(LogName).Success)
                {
                    return string.Empty;
                }
                var logcontents = string.Empty;
                var logpath =Path.Combine(@"C:\mcu-release-automation\ReleaseAutomation", string.Format(@"{0}.log", LogName));
                if (File.Exists(logpath) && !logpath.IsFileLocked())
                {
                    using (StreamReader file = File.OpenText(logpath))
                    {
                        logcontents = file.ReadToEnd();
                    }
                }
                return logcontents;
            }
        }

        public string ClassStyle { get { return "step_box_success"; } }
        #region Trans style map Indexer
        public string this[string stateKey]
        {
            get
            {
                var styleClass = "step_box_idle";
                var tuples = new List<Tuple<string, State>>();
                tuples.AddRange(Setup.ResultsTuple);
                tuples.AddRange(Processing.ResultsTuple);
                tuples.AddRange(Publish.ResultsTuple);
                
                foreach (var tuple in tuples)
                {
                    if(tuple.Item1 == stateKey)
                    {
                        var status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), tuple.Item2.status);
                        switch (status)
                        {
                            case TransactionStatus.Progress:
                                styleClass = "step_box_progress";
                                break;
                            case TransactionStatus.Completed:
                                styleClass = "step_box_success";
                                break;
                            case TransactionStatus.Failed:
                                styleClass = "step_box_failed";
                                break;
                            case TransactionStatus.Aborted:
                                styleClass = "step_box_aborted";
                                break;
                            case TransactionStatus.Idle:
                            default:
                                styleClass = "step_box_idle";
                                break;
                        }
                        break;
                    }
                }
                return styleClass;
            }
        }
        #endregion
        [JsonIgnore]
        public string [] StateTags
        {
            get
            {
                var statetagList = new List<string>();
                statetagList.AddRange(Setup.StateTags);
                statetagList.AddRange(Processing.StateTags);
                statetagList.AddRange(Publish.StateTags);
                return statetagList.ToArray();
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
        [JsonIgnore]
        public string[] StateTags
        {
            get
            {
                var tagList = new List<string>();
                foreach(var tuple in ResultsTuple)
                {
                    tagList.Add(tuple.Item1);
                }
                return tagList.ToArray();
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

    [Serializable] public class git_set_proxy_state : State { }
    [Serializable] public class git_config_user_state : State { }
    [Serializable] public class git_config_user_email_state : State { }
    [Serializable] public class git_clone_sandbox_state : State { }
    [Serializable] public class git_query_history_for_validation_state : State { }
    [Serializable] public class git_commit_history_validation_state : State { }
    [Serializable] public class git_clone_staging_state : State { }
    [Serializable] public class git_clone_iafw_cr_tools_state : State { }
    [Serializable] public class script_validation_state : State { }
    [Serializable]
    public class SetupState
    {
        public git_set_proxy_state git_set_proxy { get; set; }
        public git_config_user_state git_config_user { get; set; }
        public git_config_user_email_state git_config_user_email { get; set; }
        public git_clone_sandbox_state git_clone_sandbox { get; set; }
        public git_query_history_for_validation_state git_query_history_for_validation { get; set; }
        public git_commit_history_validation_state git_commit_history_validation { get; set; }
        public git_clone_staging_state git_clone_staging { get; set; }
        public git_clone_iafw_cr_tools_state git_clone_iafw_cr_tools { get; set; }
        public script_validation_state script_validation { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class Setup : StateGroup
    {
        private List<SetupState> _stateList = new List<SetupState>();
        public SetupState[] States
        {
            get { return _stateList.ToArray(); }
            set { _stateList.Clear(); _stateList.AddRange((SetupState[])value); }
        }
        protected override dynamic[] _getStates() { return States; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }

    [Serializable] public class git_add_mcu_to_sandbox_state:State { }
    [Serializable] public class git_edit_inf_file_state : State { }
    [Serializable] public class edit_read_me_state : State { }
    [Serializable] public class git_commit_mcu_state : State { }
    [Serializable]
    public class ProcessingState
    {
        public git_add_mcu_to_sandbox_state git_add_mcu_to_sandbox { get; set; }
        public git_edit_inf_file_state git_edit_inf_file { get; set; }
        public edit_read_me_state edit_read_me { get; set; }
        public git_commit_mcu_state git_commit_mcu { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class Processing : StateGroup
    {
        private List<ProcessingState> _stateList = new List<ProcessingState>();
        public ProcessingState[] States
        {
            get { return _stateList.ToArray(); }
            set { _stateList.Clear(); _stateList.AddRange((ProcessingState[])value); }
        }
        protected override dynamic[] _getStates() { return States; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }


    [Serializable] public class script_prep_release_local_state : State { }
    [Serializable] public class edit_manifest_file_state : State { }
    [Serializable] public class git_query_history_for_commi_state : State { }
    [Serializable] public class git_query_history_for_export_state : State { }
    [Serializable] public class git_query_history_for_notes_state : State { }
    [Serializable] public class git_query_commit_history_state : State { }
    [Serializable] public class git_commit_state : State { }
    [Serializable] public class git_invoke_export_state : State { }
    [Serializable] public class git_generate_release_notes_state : State { }
    [Serializable] public class script_prep_release_phase_3_state : State { }
    [Serializable] public class git_push_to_github_state : State { }
    [Serializable] public class cleanup_state : State { }
    [Serializable]
    public class PublishState
    {
        public script_prep_release_local_state script_prep_release_local { get; set; }
        public edit_manifest_file_state edit_manifest_file { get; set; }
        public git_query_history_for_commi_state git_query_history_for_commit { get; set; }
        public git_query_history_for_export_state git_query_history_for_export { get; set; }
        public git_query_history_for_notes_state git_query_history_for_notes { get; set; }
        public git_query_commit_history_state git_query_commit_history { get; set; }
        public git_commit_state git_commit { get; set; }
        public git_invoke_export_state git_invoke_export { get; set; }
        public git_generate_release_notes_state git_generate_release_notes { get; set; }
        public script_prep_release_phase_3_state script_prep_release_phase_3 { get; set; }
        public git_push_to_github_state git_push_to_github { get; set; }
        public cleanup_state cleanup { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class Publish : StateGroup
    {
        private List<PublishState> _stateList = new List<PublishState>();
        public PublishState[] States
        {
            get { return _stateList.ToArray(); }
            set { _stateList.Clear(); _stateList.AddRange((PublishState[])value); }
        }

        protected override dynamic[] _getStates() { return States; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}