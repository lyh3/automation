using System;
using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class DeleteState : MergeWorkerState
    {
        #region Constructors

        public DeleteState(ILog logger) : base(logger) { }
        public DeleteState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public DeleteState( ILog logger, 
                            XpathNavigatorMerge xpathNavigatorMerge,
                            Dictionary<string, string> xpathDictionary,
                            XmlDocument xmlCombineTargetDoc = null,
                            XmlDocument xmlCombineSourceDoc = null)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }

        #endregion

        protected override void DoWork()
        {
            try
            {
                Success = _xpathNavigatorMerge.Delete(_xpathDictionary);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format(@"--- Exception caught at DleteState, error was : {0}", ex.Message));
            }
        }
    }
}
