using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public class DurationCounterAttribute : HandlerAttribute
    {
        public string Clock { get; set; }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var counterHandler = new TimerHandler(container.Resolve<IPerformanceCounterService>());
            counterHandler.Clock = Clock.Contains("Duration") ? Clock : Clock + "Duration";
            return counterHandler;
        }
    }

    public class TimerHandler : ICallHandler
    {
        private readonly IPerformanceCounterService _counterService;

        public TimerHandler(IPerformanceCounterService counterService)
        {
            _counterService = counterService;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var start = DateTime.Now.Ticks;
            var result = getNext()(input, getNext);
            var end = DateTime.Now.Ticks;
            _counterService.IncrementCounter(Clock, end - start);
            _counterService.IncrementCounter(Clock.Replace("Duration", "Base"));
            return result;
        }

        public int Order { get; set; }
        public string Clock { get; set; }
    }
}
