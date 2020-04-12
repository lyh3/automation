using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class TestGroup
    {
        public const int MAX_TEST_CASE_PER_GROUP = 999;
        public const string UNKNOWN_GROUP = "UnknownGroup";
        private List<Test> _tests = new List<Test>();

        #region Constructors
        public TestGroup()
        {
            key = UNKNOWN_GROUP;
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

        private int _checkAvailableIndex()
        {
            var x = 1;
            var exitingTestsList = new List<string>();
            #if DEBUG
            exitingTestsList.AddRange(Directory.GetFiles(@"..\..\..\workstates\CrTestState", string.Format("crHealth_{0}_*_State.py", key), SearchOption.AllDirectories));
            #else
            exitingTestsList.AddRange(Directory.GetFiles(@".\workstates\CrTestState", string.Format("crHealth_{0}_*_State.py", key), SearchOption.AllDirectories));
            #endif
            foreach(var f in exitingTestsList)
            {
                x = int.Parse(Path.GetFileName(f).Split(new Char[] { '_' })[2]);
                if (x >= exitingTestsList.Count)
                    break;
            }
            if (x <= exitingTestsList.Count)
                x = exitingTestsList.Count + 1;
            return x;
        }
        #region Public Methods
        public Test InsertNewTest()
        {
            var test = new Test();
            var idx = 0;
            var exitingTestsList = new List<string>();
            #if DEBUG
            exitingTestsList.AddRange(Directory.GetFiles(@"..\..\..\workstates\CrTestState", string.Format("crHealth_{0}_*_State.py", key), SearchOption.AllDirectories));
            #else
            exitingTestsList.AddRange(Directory.GetFiles(@".\workstates\CrTestState", string.Format("crHealth_{0}_*_State.py", key), SearchOption.AllDirectories));
            #endif
            foreach (var f in exitingTestsList)
            {
                idx = int.Parse(Path.GetFileName(f).Split(new Char[] { '_' })[2]);
                if (idx >= exitingTestsList.Count)
                    break;
            }
            var x = 0;
            foreach (var t in _tests)
            {
                var n = int.Parse(t.testid.Replace(string.Format("{0}_", key), ""));
                if (n >= idx)
                    x = n;
            }

            idx = Math.Max(idx, x);

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
            return new JsonFormatter(JsonConvert.SerializeObject(this, Formatting.Indented)).Format();
        }
#endregion
    }
}
