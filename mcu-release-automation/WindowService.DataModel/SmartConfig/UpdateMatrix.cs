using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class UpdateMatrix
    {
        private List<KeyValuePair<string, UpdateMatrixData>> _updateDataList = new List<KeyValuePair<string, UpdateMatrixData>>();
        private ConfigTreeNode _parentNode = null;
        public List<KeyValuePair<string, UpdateMatrixData>> UpdateData
        {
            get { return _updateDataList; }
            set
            {
                _updateDataList = value;
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        [JsonIgnore]
        public ConfigTreeNode ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }
    }
    [Serializable]
    public class UpdateMatrixData
    {
        private string _optionKey = string.Empty;
        private Props _properties = new Props();
        private RawData _rawData = new RawData();
        private ConfigTreeNode _parentNode = null;
        public UpdateMatrixData(string optionKey)
        {
            _optionKey = optionKey;
        }
        [JsonIgnore]
        public ConfigTreeNode ParentNode
        {
            get { return _parentNode; }
            set
            {
                _parentNode = value;
                _properties.ParentNode =_rawData.ParentNode = _parentNode;                
            }
        }
        public string OptionKey
        {
            get { return _optionKey; }
        }
        public Props Properties
        {
            get { return _properties; }
            set
            {
                _properties = value;
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        public RawData RawDataMap
        {
            get { return _rawData; }
            set
            {
                _rawData = value;
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}
