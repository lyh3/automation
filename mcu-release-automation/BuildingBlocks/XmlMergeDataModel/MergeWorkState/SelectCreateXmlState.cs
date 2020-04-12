using System;
using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class SelectCreateXmlState : SelectInsertXmlState
    {
         #region Constructors

        public SelectCreateXmlState(ILog logger) : base(logger) { }
        public SelectCreateXmlState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public SelectCreateXmlState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary,
                              XmlDocument xmlCombineTargetDoc = null,
                              XmlDocument xmlCombineSourceDoc = null)
            : this(logger, xpathNavigatorMerge, xpathDictionary) { }
        #endregion
    }
}
