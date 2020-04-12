using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public class IncrementCounterAttribute : HandlerAttribute
    {
        public string Increment { get; set; }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            //var counterHandler = container.Resolve<CounterHandler>();
            var counterHandler = new CounterHandler(container.Resolve<IPerformanceCounterService>());
            counterHandler.Increment = Increment;
            return counterHandler;
        }
    }

    internal class CounterHandler : ICallHandler
    {
        private readonly IPerformanceCounterService _counterService;

        public CounterHandler(IPerformanceCounterService counterService)
        {
            _counterService = counterService;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            _counterService.IncrementCounter(Increment);
            return getNext()(input, getNext);
        }

        public int Order { get; set; }
        public string Increment { get; set; }
    }
}
