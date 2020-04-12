using System.Diagnostics;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    using PerformanceCounter = System.Diagnostics.PerformanceCounter;

    public interface IPerformanceCounterService
    {
        PerformanceCounter Counter(string name);
        PerformanceCounter IncrementCounter(string name);
        PerformanceCounter IncrementCounter(string name, long amount);
    }

    public class PerformanceCounterService : IPerformanceCounterService
    {      
        public string Category { get; set; }

        public PerformanceCounterService() { }

        public PerformanceCounter Counter(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(Category));

            if (!string.IsNullOrEmpty(Category))
                return new PerformanceCounter { CategoryName = Category, CounterName = name, ReadOnly = false };
            else
                return null;
        }

        public PerformanceCounter IncrementCounter(string name)
        {
            return IncrementCounter(name, 1);
        }

        public PerformanceCounter IncrementCounter(string name, long amount)
        {
            var counter = Counter(name);
            if (counter != null)
                counter.IncrementBy(amount);
            return counter;
        }
    }
}
