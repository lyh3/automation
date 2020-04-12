using System;
using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class SelectState : MergeWorkerState
    {
        #region Constructors

        public SelectState(ILog logger) : base(logger) { }
        public SelectState(ILog logger, 
                           XpathNavigatorMerge xpathNavigatorMerge,
                           Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public SelectState(ILog logger, 
                           XpathNavigatorMerge xpathNavigatorMerge,
                           Dictionary<string, string> xpathDictionary,
                           XmlDocument xmlCombineTargetDoc = null,
                           XmlDocument xmlCombineSourceDoc = null)
            : this(logger, xpathNavigatorMerge, xpathDictionary) { }
        #endregion

        protected override void DoWork()
        {
            var outXml = string.Empty;
            Success = _xpathNavigatorMerge.Select(_xpathDictionary, ref outXml);
            if (Success)
                base.XmlPlaceHoder = outXml;
            else
                base.XmlPlaceHoder = null;
        }
    }
}
