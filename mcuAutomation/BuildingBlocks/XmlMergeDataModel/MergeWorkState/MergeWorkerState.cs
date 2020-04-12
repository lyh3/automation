using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using log4net;
using McAfeeLabs.Engineering.Automation.WorkerThreadModel;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    abstract public class MergeWorkerState : WorkerState
    {
        #region Declarations

        protected XpathNavigatorMerge _xpathNavigatorMerge;
        protected Dictionary<string, string> _xpathDictionary;

        #endregion

        #region Properties

        public XpathMergeRootMergesMergeAction MergeAction { get; set; }
        protected string ContentXmlKey{ get { return null != MergeAction ? MergeAction.Contents[0].Key : string.Empty;}}
        protected string ContentXmlValue { get { return null != MergeAction ? MergeAction.Contents[0].Value : string.Empty; } }
        protected Dictionary<string, string> ContentDictionary
        {
            get
            {
                Dictionary<string, string> contentDictionary = new Dictionary<string, string>();
                if (null != MergeAction)
                    MergeAction.Contents.ForEach(x => contentDictionary.Add(x.Key, x.Value));
                return contentDictionary;
            }
        }

        #endregion

        #region Constructors

        public MergeWorkerState(ILog logger) : base(logger) { }
        public MergeWorkerState(ILog logger,
                                XpathNavigatorMerge xpathNavigatorMerge)
            : this(logger)
        {
            _xpathNavigatorMerge = xpathNavigatorMerge;
        }

        public MergeWorkerState(ILog logger,
                                XpathNavigatorMerge xpathNavigatorMerge,
                                Dictionary<string, string> xpathDictionary)
            : this(logger, xpathNavigatorMerge)
        {
             _xpathDictionary = xpathDictionary;
        }

        public MergeWorkerState(ILog logger,
                                XpathNavigatorMerge xpathNavigatorMerge,
                                Dictionary<string, string> xpathDictionary,
                                XmlDocument xmlCombineTargetDoc = null,
                                XmlDocument xmlCombineSourceDoc = null)
            : this(logger, xpathNavigatorMerge, xpathDictionary)
        {
        } 

        #endregion

        #region Properties

        public XpathNavigatorMerge NavigatorMerge { get { return _xpathNavigatorMerge; } }
        public string XmlPlaceHoder { get; set; }

        #endregion
    }
}
