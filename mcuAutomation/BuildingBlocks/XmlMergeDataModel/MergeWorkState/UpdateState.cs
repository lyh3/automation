﻿using System;
using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class UpdateState : MergeWorkerState
    {
        #region Constructors

        public UpdateState(ILog logger) : base(logger) { }
        public UpdateState(ILog logger, 
                           XpathNavigatorMerge xpathNavigatorMerge,
                           Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public UpdateState(ILog logger, 
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
                Success = _xpathNavigatorMerge.Update(_xpathDictionary, ContentXmlKey);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format(@"--- Exception caught at UpdateState, error was : {0}", ex.Message));
            }
        }
    }
}
