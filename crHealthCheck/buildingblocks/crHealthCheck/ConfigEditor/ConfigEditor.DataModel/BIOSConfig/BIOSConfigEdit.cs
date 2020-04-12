using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace ConfigEditor.DataModel
{
    [Serializable]
    public class BIOSConfigEdit
    {
        private string _timeout = "300";
        private string _recuring = "15";
        private Client _client = new Client();
        private serial _serial = new serial();
        private Dictionary<string, BIOSConfig> _biosConfigDictionary = new Dictionary<string, BIOSConfig>();
        #region Properties
        [JsonIgnore]
        public Dictionary<string, BIOSConfig> BIOConfigDictionary
        {
            get { return _biosConfigDictionary; }
            set
            {
                if (null != _biosConfigDictionary)
                {
                    _biosConfigDictionary.Clear();
                    var itr = ((Dictionary<string, BIOSConfig>)value).GetEnumerator();
                    while (itr.MoveNext())
                    {
                        _biosConfigDictionary.Add(itr.Current.Key, itr.Current.Value);
                    }

                }
            }
        }
        public serial serial
        {
            get { return _serial; }
            set { _serial = value; }
        }
        public Client Client
        {
            get { return _client; }
            set { _client = value; }
        }
        public string recurring { get { return _recuring; } }
        public string timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        #endregion
        public override string ToString()
        {
            var json = (new JavaScriptSerializer()).Serialize(this);
            return new JsonFormatter(json).Format();
        }
    }
}
