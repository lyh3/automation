using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO;

using ConfigEditor.DataModel;
namespace ConfigEditor.Controls
{
    public partial class SerialInvokeControl : UserControl
    {
        public const string SELECT_SERIAL_ACTION = "--- Select serial action ---";
        private Dictionary<string, dynamic> _serInvDic = null;

        public SerialInvokeControl(string invoke)
        {
            InitializeComponent();
            this.invoke = invoke;

            #if !DEBUG
            string jsonPath = @".\Json\SerialInvoke.json";
            #else
            string jsonPath = @"C:\PythonSV\crHealthCheck\Json\SerialInvoke.json";
            #endif
            try
            {
                var json = File.ReadAllText(jsonPath);
                var javaScriptSer = new JavaScriptSerializer();
                var dic = javaScriptSer.Deserialize<dynamic>(json);
                this._serInvDic = new Dictionary<string, dynamic>();
                foreach (KeyValuePair<string, dynamic> iv in dic)
                {
                    foreach (dynamic o in iv.Value)
                    {
                        var d = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, dynamic> x in o)
                        {
                            var k = x.Key;
                            foreach (KeyValuePair<string, dynamic> i in x.Value)
                            {
                                d.Add(i.Key, i.Value);
                            }
                            _serInvDic.Add(k, d);
                        }
                    }
                }
                var itr = _serInvDic.GetEnumerator();
                var idx = 0;
                while(itr.MoveNext())
                {
                    this.comboBox.Items.Add(itr.Current.Key);
                    if (invoke == itr.Current.Key)
                    {
                        this.comboBox.SelectedIndex = idx;
                        checkBox.Checked = true;
                    }
                    idx += 1;
                }
                //this.comboBox.Items.Insert(0, SELECT_SERIAL_ACTION);
            }
            catch(Exception ex)
            { }
        }

        public string invoke { get; set; }
    }
}
