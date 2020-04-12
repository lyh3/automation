using System;
using Microsoft.Practices.Unity;
using log4net;
//using AOP.Library.Authorization.WcfAuthorizationService;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [IncrementCounter(Increment = "TestData")]
    [DurationCounter(Clock = "TestData")]
    public class TestData : ITestData
    {
        private ILog _log = LogManager.GetLogger(typeof(TestBase));
        [IncrementCounter(Increment = "PrivateDate")]
        [DurationCounter(Clock = "PrivateDate")]
        private DateTime _privateDate;//performance counter private members will not be created by reflection
        [IncrementCounter(Increment = "Date")]
        [DurationCounter(Clock = "Date")]
        private DateTime Date;

        private long hits = 0L;
        public event PropertyChangedEventHandler PropertyChanged;

        [InjectionConstructor]
        public TestData()
        {
        }

        [IncrementCounter(Increment = "Hits")]
        [DurationCounter(Clock = "Hits")]
        public long Hits
        {
            get { return hits; }
            set { hits = value; }
        }

        public string Email
        {
            get;
            set;
        }

        [IncrementCounter(Increment = "IntVal")]
        [DurationCounter(Clock = "IntVal")]
        public int IntVal
        {
            get;
            set;
        }

        public string String
        {
            get;
            set;
        }

        public float Float
        {
            get;
            set;
        }

        public long Long
        {
            get;
            set;
        }

        //[NotNullValidator]
        //public object Nullable
        //{
        //    get;
        //    set;
        //}
        //[SqlDateTimeRangeValidator]
        //public DateTime DateTime
        //{
        //    get;
        //    set;
        //}

        [IncrementCounter(Increment = "Visit")]
        [DurationCounter(Clock = "Visit")]
        public void Visit()
        {
            Hits += 1;
        }

        public void RaisePropertyChangedEvent(object source, EventArgs args)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, args);
            }
        }

        //[CatcheAttribute]
        [IncrementCounter(Increment = "TimeConsummingAdd")]
        [DurationCounter(Clock = "TimeConsummingAdd")]
        public int TimeCosummingAdd(int a, int b)
        {
            for (int i = 0; i < 100000; ++i)
                ;
            return a + b;
        }

        public void AccessSecess() { }
        public void AccessDenail() { }
    }
}
