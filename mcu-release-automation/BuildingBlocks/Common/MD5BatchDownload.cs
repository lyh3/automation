using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

namespace McAfeeLabs.Engineering.Automation.Base
{
    abstract public class MD5BatchDownload : IMD5BatchDowload
    {
        #region Declarations

        private event EventHandler _eventBatchComplete;
        private List<string> _currentBatcMD5hList = new List<string>();
        protected StringBuilder _errorMessages = new StringBuilder();

        #endregion

        #region Constructor

        [InjectionConstructor]
        public MD5BatchDownload() { }

        #endregion

        #region Properties

        public event EventHandler BatchComplete
        {
            add { _eventBatchComplete = (EventHandler)Delegate.Combine(_eventBatchComplete, value); }
            remove { _eventBatchComplete = (EventHandler)Delegate.Remove(_eventBatchComplete, value); }
        }

        public List<string> CurrentBatcMD5hList { get { return _currentBatcMD5hList; } }
        public string Uri { get; set; }
        public StringBuilder ErrorMessages 
        { 
            get { return _errorMessages; } 
            set { _errorMessages = value; } 
        }

        #endregion

        #region Public Methods

        public void SampleBatchDownload(List<string> md5List, string targetfolder, string extenion = @"zip", int batchSize = 5)
        {
            var page = 0;
            var paeSize = batchSize;
            for (; ; )
            {
                _currentBatcMD5hList.Clear();
                _currentBatcMD5hList = md5List.Page<string>(page, paeSize).ToList<string>();
                if (null == _currentBatcMD5hList || _currentBatcMD5hList.Count <= 0) break;

                SampleDownload(_currentBatcMD5hList, targetfolder, extenion);

                if (null != _eventBatchComplete)
                    _eventBatchComplete(this, null);

                page += 1;
            }
        }

        #endregion

        #region Private Methods

        private void SampleDownload(List<string> md5List,
                                    string targetfolder,
                                    string extenion)
        {
            _errorMessages.Clear();
            if (null != md5List)
                md5List.ForEach(md5 => MD5Download(md5, targetfolder, extenion));
        }

        abstract protected void MD5Download(string md5, string targetFolder, string fileExtension);

        #endregion
    }
}
