using System;
using System.Web.Script.Serialization;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class BIOSItem
    {
        public string to { get; set; }
        public string navigation { get; set; }
        public string recuring { get; set; }
        public string invoke { get; set; }
        public override string ToString()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }
}
