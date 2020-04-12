using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class Report
    {
        private string _versionCommand = "xpdimm-cli version";
        public string versionCommand
        {
            get { return _versionCommand; }
            set { _versionCommand = value; }
        }
    }
}
