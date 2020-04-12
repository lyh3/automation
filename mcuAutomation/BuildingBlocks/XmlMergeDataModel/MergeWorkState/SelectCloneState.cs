using System.Collections.Generic;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class SelectCloneState : SelectInsertXmlState
    {
        #region Constructors

        public SelectCloneState(ILog logger) : base(logger) { }
        public SelectCloneState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public SelectCloneState(ILog logger, 
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary,
                              XmlDocument xmlCombineTargetDoc = null,
                              XmlDocument xmlCombineSourceDoc = null)
            : this(logger, xpathNavigatorMerge, xpathDictionary) { }
        #endregion

        protected override void DoWork()
        {
            this.MergeAction.Contents.Clear();
            this.MergeAction.Contents.Add( new  XpathMergeRootMergesMergeActionContents{ Key = this.XmlPlaceHoder, Value = string.Empty} );
            base.DoWork();
        }
    }
}
