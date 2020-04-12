using System;
using System.Web.Script.Serialization;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class Client
    {
        public string ip { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public override string ToString()
        {
            return new JsonFormatter((new JavaScriptSerializer()).Serialize(this)).Format();
        }
    }
}
