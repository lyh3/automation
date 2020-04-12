using System;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Web.Script.Serialization;
using System.Linq;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class Test
    {
        public Test(BsonDocument doc = null)
        {
            testid = "unkown";
            objective = TestSetup = testProcedure = passFailCreterial = "";
            if (null != doc)
            {
                testid = doc.GetElement("testid").Value.AsString;
                skip = doc.GetElement("skip").Value.AsBoolean;
                objective = doc.GetElement("objective").Value.AsString;
                TestSetup = doc.GetElement("Test Setup / Preconditions").Value.AsString;
                testProcedure = doc.GetElement("testProcedure").Value.AsString;
                passFailCreterial = doc.GetElement("pass/failCreterial").Value.AsString;
                powercycle = doc.GetElement("powercycle").Value.AsBoolean;
                waitkeystroke = doc.GetElement("waitkeystroke").Value.AsBoolean;
                foreach (var c in doc.GetElement("shellCommand").Value.AsBsonArray)
                {
                    var e = c.AsBsonDocument.GetElement(0);
                    var dic = new Dictionary<string, string>();
                    dic.Add(e.Name, e.Value.AsString);
                    _shellCommandList.Add(dic);
                }
                foreach (var d in doc.GetElement("expecteddata").Value.AsBsonArray)
                {
                    var eDoc = d.AsBsonDocument;
                    var dic = new Dictionary<string, string>();
                    var e = eDoc.GetElement("name");
                    dic[e.Name] = e.Value.AsString;
                    e = eDoc.GetElement("value");
                    dic[e.Name] = e.Value.AsString;
                    _expecteddata.Add(dic);
                }
            }
        }
        private List<Dictionary<string, string>> _shellCommandList = new List<Dictionary<string, string>>();
        private List<Dictionary<string, string>> _expecteddata =new List<Dictionary<string, string>>();
        private bool _powercycle = false;
        private bool _waitkeystroke = false;

        #region Properties
        public string testid { get; set; }
        public bool skip { get; set; }
        public string objective { get; set; }
        public string TestSetup { get; set; }
        public string testProcedure { get; set; }
        public string passFailCreterial { get; set; }
        public Dictionary<string, string>[] shellCommand
        {
            get { return _shellCommandList.ToArray(); }
            set
            {
                _shellCommandList.Clear();
                _shellCommandList.AddRange(value);
            }
        }
        public Dictionary<string, string>[] expecteddata
        {
            get { return _expecteddata.ToArray(); }
            set
            {
                _expecteddata.Clear();
                _expecteddata.AddRange(value);
            }
        }

        public bool powercycle
        {
            get { return _powercycle; }
            set { _powercycle = value; }
        }
        public bool waitkeystroke
        {
            get { return _waitkeystroke; }
            set { _waitkeystroke = value; }
        }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }

        public void RemoveExpectedData(string key)
        {
            var itr = _expecteddata.GetEnumerator();
            while (itr.MoveNext())
            {
                if (itr.Current["name"] == key)
                {
                    _expecteddata.Remove(itr.Current);
                    break;
                }
            }
        }

        public void RemoveShellCommand(string command)
        {
            var dic = new Dictionary<string, string>();
            dic["command"] = command;
            if (_shellCommandList.Contains(dic))
                _shellCommandList.Remove(dic);
        }

        public bool IsExpectedDataExist(string key)
        {
            var isexists = false;
            var count = 0;
            foreach (var d in _expecteddata)
            {
                var itr = d.GetEnumerator();
                while(itr.MoveNext())
                {
                    if(itr.Current.Value == key)
                    {
                        count += 1;
                    }
                }
            }
            if(count > 1)
            {
                isexists = true;
            }
            return isexists;
        }
        public bool IsShellCommandExist(string value)
        {
            var isexists = false;
            var count = 0;
            foreach (var d in _shellCommandList)
            {
                var itr = d.GetEnumerator();
                while (itr.MoveNext())
                {
                    if (itr.Current.Value == value)
                    {
                        count += 1;
                    }
                }
            }
            if (count > 1)
            {
                isexists = true;
            }
            return isexists;
        }
        #endregion
    }
}