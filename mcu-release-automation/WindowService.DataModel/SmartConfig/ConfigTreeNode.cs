using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class ConfigTreeNode
    {
        private string _jpath = string.Empty;
        private event EventHandler _eventPropertyChanged;
        private Props _priperties = new Props();
        private List<ConfigTreeNode> _submenu = new List<ConfigTreeNode>();
        private List<string> _observerlist = new List<string>();
        private List<string> _optionlist = new List<string>();
        private RawData _rawDataMap = new RawData();
        private UpdateMatrix _updatDataSource = new UpdateMatrix();
        private string _subNodeSelectId = Guid.NewGuid().ToString().ToUpper();
        private bool _collapse = false;
        private Stream _targetBinaryStream = null;
        private ConfigNodeStatus _nodeEditStatus = ConfigNodeStatus.Idle;
        private SmartConfigDataModel _parentModel = null;
        private long _streamTotalSize = 0L;
        private string _currentSelectedSubPath = string.Empty;
        public ConfigTreeNode()
        {
        }
        public event EventHandler PropertryChanged
        {
            add { _eventPropertyChanged = (EventHandler)Delegate.Combine(_eventPropertyChanged, value); }
            remove { _eventPropertyChanged = (EventHandler)Delegate.Remove(_eventPropertyChanged, value); }
        }
        public void Reset()
        {
            _rawDataMap.Reset();
            _streamTotalSize = 0L;
            NodeEditStatus = ConfigNodeStatus.Idle.ToString();
            _targetBinaryStream = null;
            foreach (var sub in _submenu)
            {
                sub.Reset();
            }
        }
        [JsonIgnore]
        public string CurrentSelectedSubPath
        {
            get { return _currentSelectedSubPath; }
            set { _currentSelectedSubPath = value; }
        }
        [JsonIgnore]
        public string NodeEditStatus
        {
            get { return _nodeEditStatus.ToString(); }
            set
            {
                var status = ConfigNodeStatus.Idle;
                if (Enum.TryParse<ConfigNodeStatus>(value, out status))
                {
                    _nodeEditStatus = status;                   
                }
                _rawDataMap.LoadBinaryData();
            }
        }
        [JsonIgnore]
        public string SubNodeSelectId
        {
            get { return _subNodeSelectId; }
            set { _subNodeSelectId = value; }
        }
        [JsonIgnore]
        public bool Collapse
        {
            get { return _collapse; }
            set { _collapse = value; }
        }
        [JsonIgnore]
        public string JPath
        {
            get { return _jpath; }
            set { _jpath = value; }
        }
        [JsonIgnore]
        public Stream TargetBinaryStream
        {
            get { return _targetBinaryStream; }
            set
            {
                _targetBinaryStream = value;
                if (null != _targetBinaryStream && _targetBinaryStream.CanSeek)
                {
                    _rawDataMap.TargetBinaryStream = _targetBinaryStream;
                    _streamTotalSize = _targetBinaryStream.Length;
                    if (_rawDataMap.IsDataLoaded)
                    {
                        NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    else
                    {
                        NodeEditStatus = ConfigNodeStatus.Idle.ToString();
                    }

                    _targetBinaryStream = value;

                    foreach (var sub in _submenu)
                    {
                        sub.TargetBinaryStream = value;
                    }


                }
            }
        }
        [JsonIgnore]
        public long StreamTotalSize
        {
            get { return _streamTotalSize; }
            private set { _streamTotalSize = value; }
        }
        [JsonIgnore]
        public string StreamTotalSizeText
        {
            get { return _streamTotalSize == 0L ? "100" : _streamTotalSize.ToString(); }
        }
        [Display(Name = "Properties:")]
        public Props Properties
        {
            get { return _priperties; }
            set
            {
                _priperties = value;
                _priperties.ParentNode = this;
            }
        }
        [Display(Name = "Sub menu:")]
        public List<ConfigTreeNode> SubMenu{
            get { return _submenu; }
            set
            {
                _submenu.Clear();
                _submenu.AddRange(value);
            }
        }
        [Display(Name = "Raw data map:")]
        public RawData RawDataMap
        {
            get { return _rawDataMap; }
            set
            {
                _rawDataMap = value;
                _rawDataMap.ParentNode = this;
            }
        }
        [Display(Name = "Options:")]
        public string [] Options
        {
            get { return _optionlist.ToArray(); }
            set
            {
                _optionlist.Clear();
                _optionlist.AddRange(value);
            }
        }
        [Display(Name = "Observer:")]
        public List<string> Observer
        {
            get { return _observerlist; }
            set { _observerlist.Clear(); _observerlist.AddRange(value); }
        }
        [Display(Name = "Update dictionary:")]
        public UpdateMatrix UpdateDictionary
        {
            get { return _updatDataSource; }
            set
            {
                _updatDataSource = value;
                _updatDataSource.ParentNode = this;
            }
        }
        [Display(Name = "Key:")]
        public string Key { get; set; }
        [Display(Name = "Annotation:")]
        public string Annotation { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
        public void Build(string parentJpath, SmartConfigDataModel parent)
        {
            _parentModel = parent;
            _jpath = string.Format(@"{0}.{1}", parentJpath, Key);
            foreach(var x in this._submenu)
            {
                x.PropertryChanged += parent.onPropertyChange;
                x.Build(_jpath, parent);
            }
        }
        public void UpdateSettings(string json)
        {
            var updateMatrix = JsonConvert.DeserializeObject<UpdateMatrix>(json);
            if (null != _eventPropertyChanged)
            {
                _eventPropertyChanged(this, null);
            }
        }
    }
    public enum ConfigNodeStatus
    {
        Idle,
        Modified,
        Updated,
        Applied
    }
    public enum EditDataType
    {
        DataSizeEdit,
        RawDataEdit
    }
}
