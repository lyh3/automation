using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base.Extension
{
    [TestFixture]
    public class RegistryExtensionTest
    {
        private const string REG_PATH = @"HKEY_LOCAL_MACHINE\Software\McAfee\Raiden\Tool";
        private const string REG_SUBKEY = @"MetaDataX";
        private const string VALUENAME = @"Version";
        private const string VersionValue = @"9.9.9.9";

        private string MetaDataXPath { get { return string.Format(@"{0}\{1}", REG_PATH, REG_SUBKEY); } }

        [SetUp]
        public void Init()
        {
            REG_PATH.CreateRegistryKeyByPath();
        }

        [TearDown]
        public void Dispose()
        {
            var parentkey = @"Tool";
            REG_PATH.Replace(parentkey, string.Empty).DeleteSubkeyTree(parentkey);
        }

        [Test]
        public void CreateSubkey()
        {
            Assert.IsTrue(null != MetaDataXPath.CreateRegistryKeyByPath());
        }

        [Test]
        public void DeleteSubkey()
        {
            CreateSubkey();
            Assert.IsTrue(REG_PATH.DeleteSubkey(REG_SUBKEY));
        }

        [Test]
        public void SetValue()
        {
            MetaDataXPath.CreateRegistryKeyByPath();
            MetaDataXPath.SetValue(VALUENAME, VersionValue);

            Assert.AreEqual(VersionValue, MetaDataXPath.GetValue(VALUENAME));
        }

        [Test]
        public void SetValueToNotExistsKey()
        {
            var path = string.Format(@"{0}\{1}", REG_PATH, "Foo");
            Assert.IsFalse(path.SetValue(VALUENAME, VersionValue, create: false));
            Assert.IsTrue(path.SetValue(VALUENAME, VersionValue, create: true));
        }

        [Test]
        public void DeleteValue()
        {
            SetValue();
            Assert.IsTrue(MetaDataXPath.DeleteValue(VALUENAME));
        }

        [Test]
        public void TraverseRegistryPath()
        {
            for (int i = 0; i < 3; i++)
            {
                var path = string.Format(@"{0}\{1}_{2}", REG_PATH, REG_SUBKEY, i);
                path.CreateRegistryKeyByPath();
            }
            var pathes = REG_PATH.TraverseRegistryPath();
            Assert.AreEqual(4, pathes.Length);
        }
    }
}
