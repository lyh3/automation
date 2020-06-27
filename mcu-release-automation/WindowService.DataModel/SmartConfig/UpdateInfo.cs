using System;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    abstract public class UpdateInfo
    {
        protected string _group = string.Empty;
        protected string _id = string.Empty;
        public UpdateInfo(string id, string group)
        {
            _id = id;
            _group = group;
        }
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }
    }
    [Serializable]
    public class RawDataMapInfo : UpdateInfo
    {
        private RawData _rawData = new RawData();
        public RawDataMapInfo(string id, string group) : base(id, group) { }
        public RawData RawDataMap
        {
            get { return _rawData; }
            set { _rawData = value; }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class PropertiesInfo : UpdateInfo
    {
        private Props _rawData = new Props();
        //private String _selectedValue = string.Empty;
        public PropertiesInfo(string id, string group) : base(id, group) { }
        public Props Properties
        {
            get { return _rawData; }
            set { _rawData = value; }
        }
        //public string SelectedValue
        //{
        //    get { return _selectedValue; }
        //    set { _selectedValue = value; }
        //}
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}
