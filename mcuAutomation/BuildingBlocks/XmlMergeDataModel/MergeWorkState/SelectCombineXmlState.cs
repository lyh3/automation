using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class SelectCombineXmlState : SelectInsertXmlState
    {
        #region Declarations
        public const string SAMPLE_NS = "http://mcafeelabs.com/schema/automation/sample/metadata/2.0";
        private XmlDocument _xmlCombineSourceDoc;
        private XmlDocument _xmlCombineTargetDoc;

        #endregion

        #region Constructors

        public SelectCombineXmlState(ILog logger) : base(logger) { }
        public SelectCombineXmlState(ILog logger,
                              XpathNavigatorMerge xpathNavigatorMerge,
                              Dictionary<string, string> xpathDictionary)
            : base(logger, xpathNavigatorMerge, xpathDictionary) { }
        public SelectCombineXmlState(ILog logger,
                                      XpathNavigatorMerge xpathNavigatorMerge,
                                      Dictionary<string, string> xpathDictionary,
                                      XmlDocument xmlCombineTargetDoc = null,
                                      XmlDocument xmlCombineSourceDoc = null)
            : this(logger, xpathNavigatorMerge, xpathDictionary)
        {
            _xmlCombineTargetDoc = xmlCombineTargetDoc;
            _xmlCombineSourceDoc = xmlCombineSourceDoc;
        }

        #endregion

        protected override void DoWork()
        {
            char[] delimiters = new char[] { ',', ';' };
            try
            {
                var targetNodes = _xmlCombineTargetDoc.SelectNodes(_xpathDictionary.Keys.ToArray()[0]);
                var splitKeys = ContentXmlKey.Split(delimiters);
                var splitValues = ContentXmlValue.Split(delimiters);
                Debug.Assert(null != splitKeys
                            && null != splitValues
                            && splitKeys.Length == splitValues.Length);

                for (int i = 0; i < splitKeys.Length; ++i)
                {
                    foreach (XmlNode targetNode in targetNodes)
                    {
                        var xpath = string.Format(splitKeys[i], targetNode.InnerText);
                        var sourceNodes = _xmlCombineSourceDoc.SelectNodes(xpath);
                        if (null == sourceNodes || sourceNodes.Count <= 0) continue;

                        var sb = new StringBuilder();
                        foreach (XmlNode sourceNode in sourceNodes)
                            sb.AppendLine(DecodeCData(sourceNode));

                        xpath = string.Format(splitValues[i], targetNode.InnerText);
                        var xpathDictionary = new Dictionary<string, string>();
                        xpathDictionary.Add(xpath, string.Empty);
                        _xpathNavigatorMerge.SelectInsertXml(xpathDictionary, sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format(@"--- Exception caught at SelectMergeState, error was : {0}", ex.Message));
            }

            EncodeCData(_xpathNavigatorMerge.MergedXmlDoc);
        }

        private string DecodeCData(XmlNode node)
        {
            return node.OuterXml.Replace(string.Format(@"xmlns={0}{1}{0}", "\"", SAMPLE_NS), string.Empty)
                                .Replace(@"<![CDATA[", @"![CDATA[")
                                .Replace(@"]]>", @"]]");
        }

        private void EncodeCData(XmlDocument xmlDoc)
        {
            xmlDoc.LoadXml(_xpathNavigatorMerge.MergedXmlDoc.OuterXml.Replace(@">![CDATA[", @"><![CDATA[")
                                                                     .Replace(@"]]<", @"]]><"));
        }

    }
}
