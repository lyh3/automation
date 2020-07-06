using System;
using System.Collections.Generic;
using Automation.Base.BuildingBlocks;

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
        private List<string> _propertiesDefaultValue = new List<string>();
        private List<string> _propertiesCurrentValue = new List<string>();
        private List<string> _rawdataOffset = new List<string>();
        private List<string> _rawdataSize = new List<string>();
        private List<string> _rawdataValue = new List<string>();
        private List<BinaryChunk> _binaryChunk = new List<BinaryChunk>();
        private bool _isDownloadAvailable = false;
        private string _lastErrorMessage = string.Empty;

        public bool ConfigLoaded
        {
            get { return _configLoaded; }
            set { _configLoaded = value; }
        }
        public bool IsDownloadAvailable
        {
            get { return _isDownloadAvailable; }
            set { _isDownloadAvailable = value; }
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
        public List<string> PropertiesDefaultValue
        {
            get { return _propertiesDefaultValue; }
            set { _propertiesDefaultValue = value; }
        }
        public List<string> PropertiesCurrentValue
        {
            get { return _propertiesCurrentValue; }
            set { _propertiesCurrentValue = value; }
        }
        public List<string> RawDataOffset
        {
            get { return _rawdataOffset; }
            set { _rawdataOffset = value; }
        }
        public List<string> RawDataSize
        {
            get { return _rawdataSize; }
            set { _rawdataSize = value; }
        }
        public List<string> RawDataValue
        {
            get { return _rawdataValue; }
            set { _rawdataValue = value; }
        }
        public string LastErrorMessage
        {
            get { return _lastErrorMessage; }
            set { _lastErrorMessage = value; }
        }
        public List<BinaryChunk> BinaryChunk
        {
            get { return _binaryChunk; }
            set { _binaryChunk = value; }
        }
    }
}
