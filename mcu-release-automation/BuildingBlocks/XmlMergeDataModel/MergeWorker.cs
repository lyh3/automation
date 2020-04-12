using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Xml;
using System.Reflection;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class MergeWorker 
    {
        #region Declarations

        protected ConcurrentQueue<MergeWorkerState> _stateQueue = new ConcurrentQueue<MergeWorkerState>();
        protected XpathMergeRoot _xpathMergeRoot;
        protected XmlDocument _targetXmlDoc;
        protected XmlDocument _sourceXmlDoc;
        protected ILog _logger;

        #endregion

        #region Constructors

        public MergeWorker( ILog logger, 
                            XmlDocument scriptXmlDoc, 
                            XmlDocument targetXmlDoc)
        {
            _logger = logger;
            _targetXmlDoc = targetXmlDoc;
            _xpathMergeRoot = CommonUtility.XmlRetrieve<XpathMergeRoot>(scriptXmlDoc);
        }

        public MergeWorker(ILog logger,
                           XmlDocument scriptXmlDoc,
                           XmlDocument targetXmlDoc,
                           XmlDocument sourceXmlDoc)
            :this(logger, scriptXmlDoc, targetXmlDoc)
        {
            _sourceXmlDoc = sourceXmlDoc;
        }

        #endregion

        #region Public Methods

        public void Merge()
        {
            MergeWorkerState state = InitialWork();
            while (null != state)
            {
                state.Execute();
                _targetXmlDoc = state.NavigatorMerge.MergedXmlDoc;
                state = StateFactory(state);
            }
        }

        public MergeWorkerState StateFactory(MergeWorkerState args)
        {
            MergeWorkerState state = null;
            var currentState = args as MergeWorkerState;

            _stateQueue.TryDequeue(out state);
            while (null != state
                   && currentState.Success
                   && (state as MergeWorkerState).MergeAction.SkipOnLastSuccess) 
                 _stateQueue.TryDequeue(out state);

            if (null != state)
                (state as MergeWorkerState).NavigatorMerge.MergedXmlDoc = (currentState as MergeWorkerState).NavigatorMerge.MergedXmlDoc;
         
            return state;
        }

        #endregion

        #region Properties

        public XmlDocument MergedXmlDoc { get { return _targetXmlDoc; } }

        #endregion

        #region Private Methods

        protected MergeWorkerState InitialWork()
        {
            MergeWorkerState workerstate = null;

            if (!(null == _xpathMergeRoot
                || null == _xpathMergeRoot.Merges
                || null == _xpathMergeRoot.Merges.Merge))
            {
                var asm = Assembly.GetExecutingAssembly();
                var types = new List<Type>();
                types.AddRange(asm.GetTypes());

                string stateName = string.Empty;
                foreach (XpathMergeRootMergesMerge merge in _xpathMergeRoot.Merges.Merge)
                {
                    stateName = string.Format(@"{0}State", merge.Action.Name);
                    foreach (var type in types)
                    {
                        if (stateName == type.Name)
                        {
                            var navigationXpathDictionary = new Dictionary<string, string>();
                            merge.NavitationXpath.ForEach(content => navigationXpathDictionary.Add(content.LocalName, content.Namespace));
                            var instance = Activator.CreateInstance(type, new object[] { _logger, 
                                                                                     new XpathNavigatorMerge(_targetXmlDoc),
                                                                                     navigationXpathDictionary,
                                                                                     _targetXmlDoc,
                                                                                     _sourceXmlDoc
                                                                                   })
                                                                                       as MergeWorkerState;
                            if (null != instance)
                            {
                                (instance as MergeWorkerState).MergeAction = merge.Action;
                                _stateQueue.Enqueue(instance);
                            }
                        }
                    }
                }

                _stateQueue.TryDequeue(out workerstate);
            }

            return workerstate;
        }

        #endregion
    }
}
