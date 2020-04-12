using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [TestFixture]
    public class DeadLockTest : TestBase
    {
        private List<Thread> _testThreads = new List<Thread>();

        [SetUp]
        public void InitializeTest()
        {
            InitializeTestDataInstance();
            InitializeTestThreads();
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void TestDeadLockPrevent()
        {
            //_testThreads.ForEach(thread => { thread.Start(new Random(DateTime.Now.Millisecond).Next(0, 100)); });
            Parallel.ForEach(_testThreads, thread => { thread.Start(new Random(DateTime.Now.Millisecond).Next(0, 100)); });
            Assert.AreEqual(NumberOfThread, TestData.Hits);
        }

        private void InitializeTestThreads()
        {
            for (int i = 0; i < NumberOfThread; ++i)
            {
                object o = i;
                var thread = new Thread(new ParameterizedThreadStart(Work));
                thread.Name = string.Format(@"Work thread {0}", i);
                _testThreads.Add(thread);
            }
        }

        private void Work(object parameter)
        {
            var val = parameter;
            TestData.Visit();
        }
    }
}
