using System;
using System.Collections.Generic;

namespace WindowService.DataModel
{
    [Serializable]
    public class ModelEdiStatusInfo
    {
        private bool _configLoaded = false;
        private List<string> _curretnSelectedSubPath = new List<string>();
        private List<string> _rawDataEditDataType = new List<string>();
        private List<string> _rawDataClass = new List<string>();
        private List<string> _status = new List<string>();

        public bool ConfigLoaded
        {
            get { return _configLoaded; }
            set { _configLoaded = value; }
        }
        public List<string> CurrentSelectedSubPath
        {
            get { return _curretnSelectedSubPath; }
            set { _curretnSelectedSubPath.Clear(); _curretnSelectedSubPath.AddRange(value); }
        }
        public List<string> Status
        {
            get { return _status; }
            set { _status.Clear(); _status.AddRange(value); }
        }
        public List<string> RawDataEditType
        {
            get { return _rawDataEditDataType; }
            set { _rawDataEditDataType = value; }
        }
        public List<string> RawDataClass
        {
            get { return _rawDataClass; }
            set { _rawDataClass = value; }
        }
    }
}
