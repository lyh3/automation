using System;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class TestGroup
    {
        public const int MAX_TEST_CASE_PER_GROUP = 999;
        public const string UNKNOWN_GROUP = "UnknownGroup";
        private List<Test> _tests = new List<Test>();

        #region Constructors
        public TestGroup(BsonElement groupElement = null)
        {
            if (null != groupElement)
            {
                key = groupElement.Name;
                foreach (var element in groupElement.Value.AsBsonDocument.Elements)
                {
                    if (element.Name == "skip")
                    {
                        skip = element.Value.AsBoolean;
                    }
                    else if (element.Value.IsBsonArray)
                    {
                        foreach (var t in element.Value.AsBsonArray.Values)
                        {
                            _tests.Add(new Test(t.AsBsonDocument));
                        }
                    }
                }
            }
            else
            {
                key = UNKNOWN_GROUP;
            }
        }

        #endregion

        #region Properties
        public string key { get; set; }
        public bool skip { get; set; }
        [JsonIgnore]
        public string errorMessage { get; set; }
        public Test [] tests
        {
            get { return _tests.ToArray(); }
            set
            {
                _tests.Clear();
                _tests.AddRange(value);
            }
        }
        #endregion

        #region Public Methods
        public Test InsertNewTest()
        {
            var test = new Test();
            var idx = 0;
            foreach(var t in _tests)
            {
                var n = int.Parse(t.testid.Replace(string.Format("{0}_", key), ""));
                if (n >= idx)
                    idx = n;
            }
            if (idx >= MAX_TEST_CASE_PER_GROUP)
            {
                errorMessage = string.Format("The max number of test per goup is {0}.", MAX_TEST_CASE_PER_GROUP);
                return null;
            }
            test.testid = string.Format("{0}_{1}", key, (idx + 1).ToString("d3"));
            return test;
        }
        public void Add(Test test)
        {
            _tests.Add(test);
        }

        public void RemoveTest(Test test)
        {
            _tests.Remove(test);
        }

        public override string ToString()
        {
            //errorMessage = "";
            //var json = (new JavaScriptSerializer()).Serialize(this);
            //json = json.Replace("\"errorMessage\":null,", string.Empty)
            //           .Replace(",\"errorMessage\":\"\"", string.Empty);
            //return json;
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        #endregion
    }
}
