using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class Props
    {
        private List<string> _optionList = new List<string>();
        private String _default = string.Empty;
        private String _currentValue = string.Empty;
        private String _ptype = string.Empty;
        private String _selectedValue = string.Empty;
        private ConfigTreeNode _parentNode = null;
        [JsonIgnore]
        public ConfigTreeNode ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }
        [Display(Name = "Type:")]
        public string pType
        {
            get { return _ptype; }
            set
            {
                _ptype = value;
                if(null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        [Display(Name = "Default value:")]
        public String Default
        {
            get { return _default; }
            set
            {
                _default = value;
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        [Display(Name = "Current value:")]
        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        [Display(Name = "Options:")]
        public List<string> Options
        {
            get { return _optionList; }
            set
            {
                _optionList.Clear();
                _optionList.AddRange(value);
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        //[JsonIgnore]
        public string SelectedValue
        {
            get { return _selectedValue; }
            set { _selectedValue = value; }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}
