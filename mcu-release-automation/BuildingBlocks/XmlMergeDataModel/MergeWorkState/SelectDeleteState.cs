using System;
using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class SelectDeleteState : MergeWorkerState
    {
        #region Constructors

        public SelectDeleteState(ILog logger) : base(logger) { }
        public SelectDeleteState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public SelectDeleteState( ILog logger, 
                                  XpathNavigatorMerge xpathNavigatorMerge,
                                  Dictionary<string, string> xpathDictionary,
                                  XmlDocument xmlCombineTargetDoc = null,
                                  XmlDocument xmlCombineSourceDoc = null)
            : this(logger, xpathNavigatorMerge, xpathDictionary) { }
        #endregion

        protected override void DoWork()
        {
            try
            {
                Success = _xpathNavigatorMerge.SelectDelete(_xpathDictionary);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format(@"--- Exception caught at SelectDeleteState, error was : {0}", ex.Message));
            }
        }
    }
}
