using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil
{
    public class XpathNavigatorMerge
    {
        #region Declarations

        private XmlDocument _mergedXmlDoc;
        private string _lastXpathNavitatedTo = string.Empty;

        #endregion

        #region Constructor

        public XpathNavigatorMerge(XmlDocument targetXmlDoc)
        {
            _mergedXmlDoc = new XmlDocument();
            _mergedXmlDoc.LoadXml(targetXmlDoc.OuterXml);
        }

        #endregion

        #region Properties

        public XmlDocument MergedXmlDoc 
        { 
            get { return _mergedXmlDoc; }
            set { _mergedXmlDoc = value; }
        }

        public string LastXpathNavitatedTo { get { return _lastXpathNavitatedTo; } }

        #endregion

        #region Public Methods

        public bool Select(Dictionary<string, string> xpathDictionary, ref string outXml)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var node = Select(xpathNavigator, xpathDictionary);
            if (null != node)
            {
                outXml = (node as XPathNavigator).OuterXml;
                return true;
            }
            return false;
        }

        public bool InsertXml( Dictionary<string, string> xpathDictionary, 
                               string xmlInsert)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var sucess = NavigateTo(xpathNavigator, xpathDictionary);
            if(sucess)
                xpathNavigator.AppendChild(xmlInsert);
            return sucess;
        }

        public bool SelectInsertXml(Dictionary<string, string> xpathDictionary,
                                    string xmlInsert)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator(); 
            var node = Select(xpathNavigator, xpathDictionary);
            if (null != node)
            {
                node.AppendChild(xmlInsert);
                return true;
            }
            return false;
        }

        public bool Delete(Dictionary<string, string> xpathDictionary, bool backToparent = false)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var sucess = NavigateTo(xpathNavigator, xpathDictionary, backToparent);
            if (sucess)
                xpathNavigator.DeleteSelf();
            return sucess;
        }

        public bool SelectDelete(Dictionary<string, string> xpathDictionary)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var node = Select(xpathNavigator, xpathDictionary);
            if (null != node)
            {
                node.DeleteSelf();
                return true;
            }
            return false;
        }

        public bool Update(Dictionary<string, string> xpathDictionary, dynamic value)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var sucess = NavigateTo(xpathNavigator, xpathDictionary);
            if (sucess)
                xpathNavigator.SetTypedValue(value);
            return sucess;
        }

        public bool SelectUpdate(Dictionary<string, string> xpathDictionary, dynamic value)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var node = Select(xpathNavigator, xpathDictionary);
            if (null != node)
            {
                node.SetTypedValue(value);
                return true;
            }
            return false;
        }

        public bool CreateAttribute(Dictionary<string, string> xpathDictionary,
                                    Dictionary<string, string> attributeDictionary)
        {
            var xpathNavigator = _mergedXmlDoc.CreateNavigator();
            var sucess = NavigateTo(xpathNavigator, xpathDictionary);
            if (sucess)
            {
                XmlWriter attributes = xpathNavigator.CreateAttributes();

                var itr = attributeDictionary.GetEnumerator();
                while (itr.MoveNext())
                    attributes.WriteAttributeString(itr.Current.Key, itr.Current.Value);

                attributes.Close();
            }
            return sucess;
        }

        #endregion

        #region Private Methods

        private bool NavigateTo(XPathNavigator xpathNavigator,
                                Dictionary<string, string> xpathDictionary, 
                                bool backToParent = true)
        {
            var itr = xpathDictionary.GetEnumerator();
            var success = true;

            if (null != (object)itr)
            {
                _lastXpathNavitatedTo = string.Empty;
                try
                {
                    while (itr.MoveNext())
                    {
                        success &= xpathNavigator.MoveToChild(itr.Current.Key, itr.Current.Value);
                        if (success)
                            _lastXpathNavitatedTo = itr.Current.Key;
                        else
                            break;
                    }
                    if (backToParent && success)
                        xpathNavigator.MoveToParent();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format(@"--- Exception caught at XpathNavigatorMerge/NavigateTo, error was:{0}", ex.StackTrace));
                }
            }
            return success;
        }

        private dynamic Select(XPathNavigator xpathNavigator,
                                Dictionary<string, string> xpathDictionary)
        {
            dynamic node = null;
            var itr = xpathDictionary.GetEnumerator();
            if (null != (object)itr)
            {
                try
                {
                    while (itr.MoveNext())
                    {
                        node = xpathNavigator.SelectSingleNode(itr.Current.Key);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format(@"--- Exception caught at XpathNavigatorMerge/select, error was:{0}", ex.StackTrace));
                }
            }
            return node;
        }

        #endregion
    }
}
