using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor.DataModel
{
    public class ExpectedData
    {
        public ExpectedData(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
        public string name { get; set; }
        public string value { get; set; }
    }
}
