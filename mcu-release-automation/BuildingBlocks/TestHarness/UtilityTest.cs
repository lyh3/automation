using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

using NUnit.Framework;
using McAfeeLabs.Engineering.Automation.Base;

namespace McAfeeLabs.Engineering.Automation.Base.Common
{
    [TestFixture]
    public class UtilityTest : TestHarness
    {
        [Test]
        public void PingFtpSite()
        {
            CommonUtility.PingFtpSite(@"ftp://mariah.ssc.wisc.edu/pub/stf704a/AH/ ");//a public ftp site from The crest of the University of Wisconsin-MadisonUniversity of Wisconsin-Madison
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(TestFilepath))
                File.Delete(TestFilepath);
        }

        [Test]
        public void XmlDeserialize()
        {
            if (null == _assembly) Initial();
            CreateXmlTestFileFromEmbeddedStream();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(TestFilepath);
            var obj = CommonUtility.XmlRetrieve<doc>(xmlDoc);
            Assert.IsNotNull(obj);
        }

        [Test]
        public void XmlSerialize()
        {
            if(null ==  _assembly) Initial();
            CreateXmlTestFileFromEmbeddedStream();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(TestFilepath);
            var obj = CommonUtility.XmlRetrieve<doc>(xmlDoc);
            Assert.IsNotNull(obj);

            var xml = CommonUtility.XmlPersist<doc>(obj);
            Assert.IsFalse(string.IsNullOrEmpty(xml));
        }

        [Test]
        public void FibNumbersUsingTupleUnfold()
        {
            var expected = new List<int>();
            expected.AddRange(new[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 });
            var fibs = CommonUtility.Unfold(Tuple.Create(0, 1),
                                      state => Tuple.Create(state.Item1, Tuple.Create(state.Item2, state.Item1 + state.Item2)))
                                      .Take(10).ToList();
            Assert.AreEqual(expected, fibs);
        }

        [Test]
        public void EvenNumberUsingTupleUnfold()
        {
            var expected = new List<int>();
            expected.AddRange(new[] { 0, 2, 4, 6, 8 }); 
           
            var evens = CommonUtility.Unfold(0, state => state < 10 ? Tuple.Create(state, state + 2) : null)
            .ToList();
            Assert.AreEqual(expected, evens);
        }
    }
}
