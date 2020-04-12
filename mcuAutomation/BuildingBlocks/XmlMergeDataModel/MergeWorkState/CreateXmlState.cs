using System;
using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class CreateXmlState : InsertXmlState
    {
         #region Constructors

        public CreateXmlState(ILog logger) : base(logger) { }
        public CreateXmlState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public CreateXmlState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary,
                              XmlDocument xmlCombineTargetDoc = null,
                              XmlDocument xmlCombineSourceDoc = null)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }

        #endregion
    }
}
