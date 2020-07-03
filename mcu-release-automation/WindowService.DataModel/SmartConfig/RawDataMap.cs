using System;
using Newtonsoft.Json;
using System.IO;
using Automation.Base.BuildingBlocks;
using Automation.Base;

namespace WindowService.DataModel
{
    [Serializable]
    public class RawData
    {
        public const string MAX_SIZE = "0x7C000";  // 4 MB
        private string _offset = string.Empty;
        private string _size = MAX_SIZE;
        private string _value = string.Empty;
        private Stream _targetBinaryStream = null;
        private ConfigTreeNode _parentNode = null;
        private BinaryChunk _binaryChunk = null;
        private bool _isDataLoaded = false;
        private bool _dataModified = false;
        private EditDataType _editDataType = EditDataType.DataSizeEdit;
        public void Reset()
        {
            _dataModified = false;
            _isDataLoaded = false;
            _value = string.Empty;
        }
        [JsonIgnore]
        public ConfigTreeNode ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }
        [JsonIgnore]
        public bool IsDataLoaded
        {
            get { return _isDataLoaded; }
            private set { _isDataLoaded = value; }
        }
        [JsonIgnore]
        public string EditType
        {
            get { return _editDataType.ToString(); }
            set
            {
                var editType = EditDataType.DataSizeEdit;
                if (Enum.TryParse<EditDataType>(value, out editType))
                {
                    _editDataType = editType;
                }
            }
        }
        [JsonIgnore]
        public bool DataModified
        {
            get { return _dataModified; }
            private set { _dataModified = value; }
        }
        [JsonIgnore]
        public string RawDataClass
        {
            get { return _dataModified ? "rawdata_modified" : "rawdata"; }
        }
        public string Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                LoadBinaryData();
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        public string Size
        {
            get { return _size; }
            set
            {
                var size = value;
                if (size.HexToInt() > MAX_SIZE.HexToInt())
                {
                    size = MAX_SIZE;
                }
                _size = size;
                LoadBinaryData();
                if (null != _parentNode)
                {
                    _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                }
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value.TrimStart().TrimEnd();
                switch (_editDataType)
                {
                    case EditDataType.RawDataEdit:
                        if (!string.IsNullOrEmpty(_value))
                        {
                            var split = _value.Split(' ');
                            this._size = split.Length.IntToHexString();
                            _dataModified = true;
                        }
                        break;

                    case EditDataType.DataSizeEdit:
                    default:
                        if (null != _parentNode)
                        {
                            _parentNode.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                        }
                        break;
                }
            }
        }
        [JsonIgnore]
        public Stream TargetBinaryStream
        {
            get { return _targetBinaryStream; }
            set
            {
                _targetBinaryStream = value;
                this._editDataType = EditDataType.DataSizeEdit;
                LoadBinaryData();
            }
        }
        public void LoadBinaryData()
        {
            if (this._editDataType == EditDataType.RawDataEdit)
            {
                return;
            }
            if (!string.IsNullOrEmpty(_offset) && !string.IsNullOrEmpty(_size))
            {
                if (null == _targetBinaryStream || !_targetBinaryStream.CanSeek)
                    return;
                int? offset = _offset.HexToInt();
                int? size = _size.HexToInt();
                long? total = null;
                total = _targetBinaryStream.Length;
                if (offset <= total.Value && size < total.Value)
                {
                    var buffer = _targetBinaryStream.ReadBytesFromStram(offset.Value, size.Value).ToArray();
                    _binaryChunk = new BinaryChunk(buffer);
                    this.Value = _binaryChunk.ToString();
                    this._isDataLoaded = true;
                }
                else
                {
                    _isDataLoaded = false;
                }
            }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}
