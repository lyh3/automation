using System;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public class ValidationException : Exception
    {
        private readonly IMethodInvocation _input;

        public ValidationException(string message, IMethodInvocation input)
            : base(message)
        {
            _input = input;
        }

        public override string Message
        {
            get
            {
                return string.Format(@"Validation fault: [{0}] at [{1}].", base.Message, _input.MethodBase.Name.Replace(@"set_", string.Empty).Replace(@"get_", string.Empty));
            }
        }
    }
}
