using System;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public class SynchronizeAttribute : HandlerAttribute
    {
        public int FromMilliseconds { get; set; }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var callHandler = container.Resolve<SynchronizeCallHandler>();
            callHandler.FromMilliseconds = FromMilliseconds;

            return callHandler;
        }
    }

    internal class SynchronizeCallHandler : ICallHandler
    {
        private static TimedLock timeLock;
        protected int order = 0;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        [DefaultValue(500)]
        public int FromMilliseconds { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn methodReturn;
            try
            {
                timeLock = TimedLock.Lock(input.Target, new TimeSpan(0,0,0,0,FromMilliseconds));
                methodReturn = getNext().Invoke(input, getNext);
            }
            finally
            {
                timeLock.Dispose();
            }

            return methodReturn;
        }
    }
}
