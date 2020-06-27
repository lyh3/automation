using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class SmartConfigDataModel : IDisposable
    {
        private static object _updateModelSyncObj = new object();
        protected string _title = @"Smart Configuration (- under construction -)";
        protected string _jsonConfig = string.Empty;
        protected string _targetFile = string.Empty;
        protected string _resultsFile = string.Empty;
        protected string _lastErrorMessage = string.Empty;
        protected bool _configLoaded = false;
        protected bool _binaryLoaded = false;
        protected Stream _sourceStream = null;
        protected long _binarySize = 0L;
        protected List<ConfigTreeNode> _subMenu = new List<ConfigTreeNode>();

        [JsonIgnore]
        [Required(ErrorMessage = "* Please select the Json config.")]
        [Display(Name = "Json config file:")]
        public string JsonConfig
        {
            get { return _jsonConfig; }
            set { _jsonConfig = value; }
        }
        [JsonIgnore]
        [Required(ErrorMessage = "* Please select the target binary file.")]
        [Display(Name = "Target binary file:")]
        public string TargetBinaryFile
        {
            get { return _targetFile; }
            set
            {
                if (null != _sourceStream && _sourceStream.CanSeek)
                {
                    _sourceStream.Close();
                }
                if (_binaryLoaded && string.IsNullOrEmpty(value))
                {
                    if (File.Exists(_targetFile))
                    {
                        foreach(ConfigTreeNode sub in _subMenu)
                        {
                            sub.Reset();
                        }
                        File.Delete(_targetFile);
                    }
                    _binaryLoaded = false;
                }
                _targetFile = value;
                if (File.Exists(_targetFile))
                {
                    var fileInfo = new FileInfo(value);
                    _binarySize = fileInfo.Length;
                    _sourceStream = File.OpenRead(_targetFile);
                    foreach (var sub in _subMenu)
                    {
                        sub.TargetBinaryStream = _sourceStream;
                    }
                    _binaryLoaded = true;
                }
            }
        }
        [JsonIgnore]
        public long BinarySize
        {
            get { return _binarySize; }
            private set { _binarySize = value; }
        }
        [JsonIgnore]
        public Stream SourceStream
        {
            get { return _sourceStream; }
            set { _sourceStream = value; }
        }
        [JsonIgnore]
        public bool ConfigLoaded
        {
            get { return _configLoaded; }
            set { _configLoaded = value; }
        }
        [JsonIgnore]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [JsonIgnore]
        public string ResultsFile
        {
            get { return _resultsFile; }
            set
            {
                _resultsFile = value;
            }
        }
        [JsonIgnore]
        public string LastErrorMessage
        {
            get { return _lastErrorMessage; }
            set { _lastErrorMessage = value; }
        }
        public List<ConfigTreeNode> Root
        {
            get { return _subMenu; }
            set
            {
                _subMenu.Clear();
                _subMenu.AddRange(value);
                _subMenu.Reverse();
            }
        }
        public void BuildTree()
        {
            foreach (var x in _subMenu)
            {
                x.Build("Root", this);
            }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
        public void onPropertyChange(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(ConfigTreeNode))
            {
                foreach (var jpath in ((ConfigTreeNode)sender).Observer)
                {
                    var target = this.GetConfigNodeByJPath(jpath);
                    target.SyncUpdate(sender as ConfigTreeNode);
                }
            }
        }
        public void TransactionStatus(ConfigNodeStatus from, ConfigNodeStatus to)
        {
            foreach (var r in this.Root)
            {
                if (r.NodeEditStatus == from.ToString())
                {
                    r.NodeEditStatus = to.ToString();
                }
                foreach (var n in r.SubMenu)
                {
                    if (n.NodeEditStatus == from.ToString())
                    {
                        n.NodeEditStatus = to.ToString();
                    }
                    _TransactionStatus(from, to, n.SubMenu);
                }
            }
        }
        public void _TransactionStatus(ConfigNodeStatus from, ConfigNodeStatus to, IEnumerable<ConfigTreeNode> elements)
        {
            foreach (var x in elements)
            {
                if (x.NodeEditStatus == from.ToString())
                {
                    x.NodeEditStatus = to.ToString();
                }
                _TransactionStatus(from, to, x.SubMenu);
            }
        }
        public ConfigTreeNode GetConfigNodeByJPath(string jpath)
        {
            ConfigTreeNode results = null;
            foreach (var x in this._subMenu)
            {
                if (null != results)
                {
                    break;
                }
                if (x.JPath == jpath)
                {
                    results = x;
                    break;
                }
                _searchByJpath(jpath, x.SubMenu, ref results);
            }
            return results;
        }

        public ConfigTreeNode GetConfigNodeByGuid(string guid)
        {
            ConfigTreeNode results = null;
            foreach (var x in this._subMenu)
            {
                if (x.SubNodeSelectId == guid)
                {
                    results = x;
                    break;
                }
                if (null != results)
                {
                    break;
                }
                _searchByGuid(guid, x.SubMenu, ref results);
            }
            return results;
        }
        private void _searchByGuid(string guid, IEnumerable<ConfigTreeNode> elements, ref ConfigTreeNode results)
        {
            if (null != results)
            {
                return;
            }
            foreach (var x in elements)
            {
                if (null != results)
                {
                    return;
                }
                if (x.SubNodeSelectId == guid)
                {
                    results = x;
                    break;
                }
                _searchByGuid(guid, x.SubMenu, ref results);
            }
        }
        private void _searchByJpath(string jpath, IEnumerable<ConfigTreeNode> elements, ref ConfigTreeNode results)
        {
            if (null != results)
            {
                return;
            }
            foreach (var x in elements)
            {
                if (null != results)
                {
                    return;
                }
                if (x.JPath == jpath)
                {
                    results = x;
                    break;
                }
                _searchByJpath(jpath, x.SubMenu, ref results);
            }
        }
        public bool IsDownloadAvailable
        {
            get
            {
                return !string.IsNullOrEmpty(_resultsFile)
                       && File.Exists(_resultsFile)
                       && (new FileInfo(_resultsFile)).Length > 0;
            }
        }
        public void Dispose()
        {
            string[] files = { this.JsonConfig, this.TargetBinaryFile };
            foreach (var f in files)
            {
                if (File.Exists(f))
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch { }
                }
            }
        }
    }
}