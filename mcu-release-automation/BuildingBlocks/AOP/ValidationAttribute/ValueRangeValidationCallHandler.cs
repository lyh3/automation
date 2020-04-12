using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    internal abstract class ValidationCallHandler : ICallHandler
    {
        protected int order = 0;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            if (Validate(input))
            {
                IMethodReturn methodReturn = getNext().Invoke(input, getNext);
                return methodReturn;
            }
            return null;
        }

        abstract protected bool Validate(IMethodInvocation input);
    }
}
