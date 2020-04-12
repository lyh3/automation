using System.Reflection;
using System.Diagnostics;

using Microsoft.Practices.Unity;
using McAfeeLabs.Engineering.Automation.Base.FTPClient;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    using NUnit.Framework;

    [TestFixture]
    public class InstallPerformanceCountersTest : TestBase
    {
        private TestData _testData;

        [SetUp]
        public void InitializeTest()
        {
            _testData = container.Resolve<TestData>();// or new TestData();
        }

        [Test]
        public void InstallDefaultPerformanceCounters()
        {
            if (null == _testData) InitializeTest();
            PerformanceCounterInstall.CreateCounters(_testData.GetType(), Assembly.GetAssembly(typeof(TestData)), PerformanceCounterInstall.DefaultPerformanceCounterCategory);
            Assert.IsTrue(PerformanceCounterCategory.Exists(PerformanceCounterInstall.DefaultPerformanceCounterCategory));
        }

        [Test]
        [Ignore()]
        public void RemoveDefaultPerformanceCounters()
        {
            PerformanceCounterInstall.RemovePerformanceCounterCategory(PerformanceCounterInstall.DefaultPerformanceCounterCategory, Assembly.GetAssembly(typeof(TestData)));
            Assert.IsFalse(PerformanceCounterCategory.Exists(PerformanceCounterInstall.DefaultPerformanceCounterCategory));
        }

        [Test]
        public void InstallFtpFilePerformanceCounters()
        {
            PerformanceCounterInstall.CreateCounters(typeof(FTPFile), Assembly.GetAssembly(typeof(FTPClient)), FTPFile.PerfofmanceCounterCategory);
            Assert.IsTrue(PerformanceCounterCategory.Exists(FTPFile.PerfofmanceCounterCategory));
        }

        [Test]
        [Ignore()]
        public void RemoveFtpFilePerformanceCounters()
        {
            PerformanceCounterInstall.RemovePerformanceCounterCategory(FTPFile.PerfofmanceCounterCategory, Assembly.GetAssembly(typeof(FTPClient)));
            Assert.IsFalse(PerformanceCounterCategory.Exists(FTPFile.PerfofmanceCounterCategory));
        }

        [Test]
        public void InstallFtpFolderPerformanceCounters()
        {
            PerformanceCounterInstall.CreateCounters(typeof(FTPFolder), Assembly.GetAssembly(typeof(FTPClient)), FTPFolder.PerfofmanceCounterCategory);
            Assert.IsTrue(PerformanceCounterCategory.Exists(FTPFolder.PerfofmanceCounterCategory));
        }

        [Test]
        [Ignore()]
        public void RemoveFtpFolderPerformanceCounters()
        {
            PerformanceCounterInstall.RemovePerformanceCounterCategory(FTPFolder.PerfofmanceCounterCategory, Assembly.GetAssembly(typeof(FTPClient)));
            Assert.IsFalse(PerformanceCounterCategory.Exists(FTPFolder.PerfofmanceCounterCategory));
        }
    }
}
