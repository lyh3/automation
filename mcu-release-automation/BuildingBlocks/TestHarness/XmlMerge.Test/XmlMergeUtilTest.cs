using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.IO;

using NUnit.Framework;

using McAfeeLabs.Engineering.Automation.Base.XmlMergeUtil;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [TestFixture]
    public class XmlMergeUtilTest : TestHarness
    {
        const string EmbeddedSourceFileFormat = @"McAfeeLabs.Engineering.Automation.Base.EmbeddedTestData.{0}";
        private XmlDocument _sourceXmlDoc = new XmlDocument();
        private XpathNavigatorMerge _xpathNavigatorMerge;
        private Dictionary<string, string> _xpathDictionary = null;
        private XpathNavigatorMerge _testConfigXpathNavigatorMerge = null;

        [SetUp]
        public void InitializeTest()
        {
            var xmlDoc = new XmlDocument();
            var testFileName = string.Format(EmbeddedSourceFileFormat, @"BookStore.xml");

            testFileName.LoadEmbeddedXml(xmlDoc, Assembly.GetExecutingAssembly());

            _xpathNavigatorMerge = new XpathNavigatorMerge(xmlDoc);
            _xpathDictionary = new Dictionary<string, string>();
            _xpathDictionary.Add("bookstore", "http://www.contoso.com/books");
            _xpathDictionary.Add("book", "http://www.contoso.com/books");
            InitializeConfigXpathNavigatorMerge();
        }

        [Test]
        public void InsertXmlTest()
        {
            _xpathDictionary.Add("price", "http://www.contoso.com/books");

            _xpathNavigatorMerge.InsertXml(_xpathDictionary, @"<pages>100</pages>");
        }

        [Test]
        public void DeletelTest()
        {
            _xpathDictionary.Add("title", "http://www.contoso.com/books");//or xpathDictionary.Add("author", "http://www.contoso.com/books") or xpathDictionary.Add("price", "http://www.contoso.com/books")

            _xpathNavigatorMerge.Delete(_xpathDictionary);
        }

        [Test]
        public void UpdateTest()
        {
            _xpathDictionary.Add("price", "http://www.contoso.com/books");

            var newval = 7.99;
            _xpathNavigatorMerge.Update(_xpathDictionary, newval);
        }

        [Test]
        public void CreateAttributeTest()
        {
            _xpathDictionary.Add("price", "http://www.contoso.com/books");
            var attributesDictionary = new Dictionary<string, string>();
            attributesDictionary.Add("discount", "1.00");
            attributesDictionary.Add("currency", "USD");
            _xpathNavigatorMerge.CreateAttribute(_xpathDictionary, attributesDictionary);
        }

        [Test]
        public void SelectInsertXmlTest()
        {
            var success = _testConfigXpathNavigatorMerge.SelectInsertXml(CreatNavigationDictionary("/*[local-name()='configuration']/*[local-name()='unity']"), "<assembly name='Hello'/>");
            Assert.IsTrue(success);
        }
        [Test]
        public void SelectDeleteTest()
        {
            var success = _testConfigXpathNavigatorMerge.SelectDelete( CreatNavigationDictionary("/*[local-name()='configuration']/*[local-name()='configSections']/*[local-name()='section'][@name='unity']"));
            Assert.IsTrue(success);
            success = _testConfigXpathNavigatorMerge.SelectDelete(CreatNavigationDictionary("/configuration/system.serviceModel/bindings/basicHttpBinding/binding[@name='SampleManagementServiceSoap']"));
            Assert.IsTrue(success);
        }

        [Test]
        public void SelectUpdateTest()
        {
            var success = _testConfigXpathNavigatorMerge.SelectUpdate(CreatNavigationDictionary("/configuration/configSections/section[@name='unity']/@type"),
                                                                                                "Hello, this is a wrong type");
            Assert.IsTrue(success);
        }

        [Test]
        public void MergeTest()
        {
            var targetXmlDoc = LoadEmbeddedXmlResource(@"MergeTarget.config"); 
            var scriptXmlDoc = LoadEmbeddedXmlResource(@"XpathMerge.xml");

            var mergeworker = new MergeWorker(null, scriptXmlDoc, targetXmlDoc);
            mergeworker.Merge();
        }

        [Test]
        public void SandBoxMergeTest()
        {
            var targetXmlDoc = LoadEmbeddedXmlResource(@"SandboxReplicationFeed.config");
            var scriptXmlDoc = LoadEmbeddedXmlResource(@"SandboxXpathMerge.xml");

            var mergeworker = new MergeWorker(null, scriptXmlDoc, targetXmlDoc);
            mergeworker.Merge();
        }

        [Test]
        public void MetaDataMergeTest()
        {
            var scriptXmlDoc = LoadEmbeddedXmlResource(@"XpathCombine.xml");
            var medataDataSourceXmlDoc = LoadEmbeddedXmlResource(@"MetadataSourceSamplet.xml");
            var medataDataTargetXmlDoc = LoadEmbeddedXmlResource(@"MetadataTargetSamplet.xml");
            
            var mergeworker = new MergeWorker(null, 
                                              scriptXmlDoc,
                                              medataDataTargetXmlDoc, 
                                              medataDataSourceXmlDoc);
            mergeworker.Merge();
        }

        #region Private Methods

        private void InitializeConfigXpathNavigatorMerge()
        {
            var configXmlDoc = LoadEmbeddedXmlResource(@"SandboxReplicationFeed.config");
           
            _testConfigXpathNavigatorMerge = new XpathNavigatorMerge(configXmlDoc);
        }

        private static XmlDocument LoadEmbeddedXmlResource(string xmlFileName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var configXmlDoc = new XmlDocument();
            var testFileName = string.Format(EmbeddedSourceFileFormat, xmlFileName);
            testFileName.LoadEmbeddedXml(configXmlDoc, asm);
            return configXmlDoc;
        }

        private Dictionary<string, string> CreatNavigationDictionary( string key, string value = "")
        {
            var dictionary = new Dictionary<string, string>();

            dictionary.Add(key, value);
 
            return dictionary;
        }

        #endregion
    }
}
