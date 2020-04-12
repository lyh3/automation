using System;
using System.Collections.Generic;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    [Serializable]
    public class ReleaseOrchstrationModel
    {
        protected string _title = string.Empty;
        protected SupportedPlatform _selectedPlatform = SupportedPlatform.Whitley;
        private Dictionary<SupportedPlatform, PlatformInfo> _releseSourceDictionary = new Dictionary<SupportedPlatform, PlatformInfo>();
        private BiosReleaseOrchestrationThread _workthread = null;

        public ReleaseOrchstrationModel()
        {
            _releseSourceDictionary.Add(SupportedPlatform.Whitley, new WhitleyPlatform());
            _releseSourceDictionary.Add(SupportedPlatform.Mehlow, new MehlowPlatform());
            _releseSourceDictionary.Add(SupportedPlatform.Purley, new PurleyPlatform());
            _releseSourceDictionary.Add(SupportedPlatform.Backerville, new BackervillePlatform());
            _releseSourceDictionary.Add(SupportedPlatform.Jacobsville, new JacobsvillePlatform());
            _releseSourceDictionary.Add(SupportedPlatform.Willsonville, new WillsonvillePlatform());
        }
        #region indexer
        public PlatformInfo this[string platform]
        {
            get
            {
                var key = (SupportedPlatform)Enum.Parse(typeof(SupportedPlatform), platform);
                if( _releseSourceDictionary.ContainsKey(key))
                {
                    return _releseSourceDictionary[key];
                }
                return null;
            }
        }
        #endregion
        public BiosReleaseOrchestrationThread Workthread
        {
            get { return _workthread; }
            set { _workthread = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string SelectedPlatform
        {
            get{return _selectedPlatform.ToString();}
            set{ _selectedPlatform = (SupportedPlatform)Enum.Parse(typeof(SupportedPlatform), value); }
        }
        public bool ProcessFailed { get; set; }
    }
}