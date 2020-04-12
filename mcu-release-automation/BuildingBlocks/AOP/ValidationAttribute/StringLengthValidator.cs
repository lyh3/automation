using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringLengthValidator : HandlerAttribute
    {
        #region Properties

        public int MaxVal { get; set; }
        public int MinVal { get; set; }

        #endregion

        #region Public Methods

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var callHandler = container.Resolve<StringLengthValidationCallHandler>();
            callHandler.MaxVal = MaxVal;
            callHandler.MinVal = MinVal;

            return callHandler;
        }

        #endregion
    }

    internal class StringLengthValidationCallHandler : ValidationCallHandler
    {
        #region Properties

        public int MaxVal { get; set; }
        public int MinVal { get; set; }

        #endregion

        #region Protected Methods

        override protected bool Validate(IMethodInvocation input)
        {
            bool isValid = true;

            if (input.Arguments.Count > 0 && null != input.Arguments[0])
            {
                string value = (string)input.Arguments[0];
                if (value.Length < MinVal)
                    throw new ValidationException(string.Format(@"The length <{0}> of string was less than minimum <{1}>", value.Length, MinVal),
                                                  input);
                if (value.Length > MaxVal)
                    throw new ValidationException(string.Format(@"The length <{0}> of string was greater than  maximum <{1}>", value.Length, MaxVal),
                                                  input);
            }

            return isValid;
        }

        #endregion
    }
}
