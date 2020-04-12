using System;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RegExValidatorAttribute : HandlerAttribute
    {
        #region Properties

        public string RegularExpression { get; set; }

        #endregion

        #region Public Method

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var callHandler = container.Resolve<RegExValidationCallHandler>();
            callHandler.RegularExpression = RegularExpression;

            return callHandler;
        }

        #endregion
    }

    internal class RegExValidationCallHandler : ValidationCallHandler
    {
        #region Properties

        public string RegularExpression { get; set; }

        #endregion

        #region Protected Methods

        override protected bool Validate(IMethodInvocation input)
        {
            bool isValid = true;

            if (input.Arguments.Count > 0 && null != input.Arguments[0])
            {
                string value = (string)input.Arguments[0];
                if (string.IsNullOrEmpty(value))
                    throw new ValidationException(string.Format(@"Regular expression is empty", value, RegularExpression),
                                                  input);
                if (!Regex.IsMatch(value, RegularExpression))
                    throw new ValidationException(string.Format(@"Value< {0}> doex not match the specified regular expression <{1}>", value, RegularExpression),
                                              input);

            }

            return isValid;
        }

        #endregion
    }
}
