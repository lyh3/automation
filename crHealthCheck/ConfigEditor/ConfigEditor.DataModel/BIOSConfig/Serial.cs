using System;
using System.Web.Script.Serialization;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class serial
    {
        public string port { get; set; }
        public string baud { get; set; }
        public override string ToString()
        {
            var json = (new JavaScriptSerializer()).Serialize(this);
            return new JsonFormatter(json).Format();
        }
    }
}
