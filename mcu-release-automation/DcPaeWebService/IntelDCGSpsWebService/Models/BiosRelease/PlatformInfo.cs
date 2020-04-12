using System;
using System.Collections.Generic;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    abstract public class PlatformInfo
    {
        protected SupportedPlatform? _platform = null;
        protected List<string> _releaseList = new List<string>();
        protected string _selectedRelease = string.Empty;

        public PlatformInfo()
        {
            Initialize();
            if (_releaseList.Count > 0)
            {
                _selectedRelease = _releaseList[0];
            }
        }
        public string Platform { get { return _platform.ToString(); } }
        public string SelectedRelease
        { 
            get { return _selectedRelease; }
            set { _selectedRelease = value; }
        }
        public List<string> Releases
        {
            get { return _releaseList; }
        }
        abstract protected void Initialize();
    }
}