﻿using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IntegerRangeValidatorAttribute : HandlerAttribute
    {
        #region Properties

        public int MaxVal { get; set; }
        public int MinVal { get; set; }

        #endregion

        #region Public Methods

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            //var callHandler = new IntegerValueRangeValidationCallHandler()
            //{
            //    MaxVal = MaxVal,
            //    MinVal = MinVal
            //};
            var callHandler = container.Resolve<IntegerValueRangeValidationCallHandler>();
            callHandler.MaxVal = MaxVal;
            callHandler.MinVal = MinVal;

            return callHandler;
        }

        #endregion
    }

    internal class IntegerValueRangeValidationCallHandler : ValidationCallHandler
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
                int value = (int)input.Arguments[0];
                if (value.CompareTo(MinVal) < 0)
                    throw new ValidationException(string.Format(@"Scalar value <{0}> was below minimum <{1}>", value, MinVal),
                                                  input);
                if (value.CompareTo(MaxVal) > 0)
                    throw new ValidationException(string.Format(@"Scalar value <{0}> was above maximum<{1}>", value, MaxVal),
                                                  input);
            }

            return isValid;
        }

        #endregion
    }
}
