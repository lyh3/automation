using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class HealthCheck
    {
        public const string DEFAULT_JSON_FILE_NAME = "AEPHealthCheck.json";
        private Dictionary<string, TestGroup> _testGroupDictionary = new Dictionary<string, TestGroup>();
        private string _timeout = "300";
        private string _recurring = "15";
        private string _reporthtmlfilename = "HealthCheckReport.html";
        private string _serialportbaudrate = "115200";
        private string _serialport = "";
        private Client _client = new Client();
        private Report _report = new Report();

        #region Indexer
        public TestGroup this[string key]
        {
            get
            {
                if(string.IsNullOrEmpty(key) || !_testGroupDictionary.ContainsKey(key))
                {
                    return null;
                }
                return _testGroupDictionary[key];
            }
        }
        #endregion

        #region Properties
        [JsonIgnore]
        public Dictionary<string, TestGroup> TestDictionary
        {
            get { return _testGroupDictionary; }
            set
            {
                if (null != _testGroupDictionary)
                {
                    _testGroupDictionary.Clear();
                    var itr = ((Dictionary<string, TestGroup>)value).GetEnumerator();
                    while(itr.MoveNext())
                    {
                        _testGroupDictionary.Add(itr.Current.Key, itr.Current.Value);
                    }

                }
            }
        }
        public string timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        public string recurring
        {
            get { return _recurring; }
            set { _recurring = value; }
        }
        public string reporthtmlfilename
        {
            get { return _reporthtmlfilename; }
            set { _reporthtmlfilename = value; }
        }
        public string serialport
        {
            get { return _serialport; }
            set { _serialport = value; }
        }
        public string serialportbaudrate
        {
            get { return _serialportbaudrate; }
            set { _serialportbaudrate = value; }
        }
        public Client Client
        {
            get { return _client; }
            set { _client = value; }
        }
        public Report Report
        {
            get { return _report; }
            set { _report = value; }
        }
        [JsonIgnore]
        public string errorMessage { get; set; }
        public void RemoveTestGroup(TestGroup group)
        {
            if (_testGroupDictionary.ContainsKey(group.key))
            {
                _testGroupDictionary.Remove(group.key);
            }
        }
        #endregion

        #region Public Methods
        public TestGroup InsertNewTestGroup()
        {
            var testGroup = new TestGroup();
            if (_testGroupDictionary.ContainsKey(testGroup.key))
            {
                errorMessage = string.Format("The group [{0}] alread exists.", testGroup.key);
                return null;
            }
            _testGroupDictionary.Add(testGroup.key, testGroup);
            return testGroup;
        }

        public override string ToString()
        {
            errorMessage = "";
            var json = (new JavaScriptSerializer()).Serialize(this);
            json = json.Replace("\"errorMessage\":null,", string.Empty)
                       .Replace(",\"errorMessage\":\"\"", string.Empty)
                       .Replace("{\"TestDictionary\":", string.Empty)
                       .Replace("},\"timeout", ",\"timeout")
                       .Replace("TestSetup", "Test Setup / Preconditions")
                       .Replace("passFailCreterial", "pass/failCreterial");
            return new JsonFormatter(json).Format();
        }
        #endregion
    }
}
