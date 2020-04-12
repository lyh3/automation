using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [TestFixture]
    public class PropertyChangedNotificationTest : TestBase
    {
        private TestDataClient testDataClient;

        public PropertyChangedNotificationTest() : base() { }

        [SetUp]
        public void Initicalization()
        {
            InitializeTestDataInstance();

            testDataClient = new TestDataClient(TestData)
            {
                NotificationReceived = false
            };
        }

        [Test]
        public void TestShouldReceiveNotificatation()
        {
            var val = 10f;
            TestData.Float = val;
            Assert.IsTrue(testDataClient.NotificationReceived);
        }

        //Should not receive notification since the String property does not attach the PropertyChangedNotification attribute
        [Test]
        public void TestShouldNotReceiveNotificatation()
        {
            var val = "Hello";
            TestData.String = val;
            Assert.IsFalse(testDataClient.NotificationReceived);
        }
    }

    public class TestDataClient
    {
        public bool NotificationReceived { get; set; }

        public TestDataClient(INotifyPropertyChanged source)
        {
            source.PropertyChanged += OnPropertyChanged;
        }

        void OnPropertyChanged(object source, EventArgs args)
        {
            NotificationReceived = true;
        }
    }
}
