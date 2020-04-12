using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class BIOSConfig
    {
        private List<BIOSItem> _biosItemList = new List<BIOSItem>();
        public BIOSItem[] navs
        {
            get { return _biosItemList.ToArray(); }
            set
            {
                _biosItemList.Clear();
                _biosItemList.AddRange(value);
            }
        }
        public override string ToString()
        {
            return new JsonFormatter((new JavaScriptSerializer()).Serialize(this)).Format();
        }
    }
}
