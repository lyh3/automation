using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyChangedNotificationAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return container.Resolve<PropertyChangedNotificationCallHandler>();
        }
    }

    internal class PropertyChangedNotificationCallHandler : ICallHandler
    {
        protected int order = 0;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn methodReturn = null;
            if (input.Arguments.Count > 0
                && null != input.Arguments[0])
            {
                object oldValue = null, newValue = null;
                if (ShouldRaiseEvent(input, ref oldValue, ref newValue))
                {
                    methodReturn = getNext()(input, getNext);
                    if (input.Target is INotifyPropertyChanged
                        && methodReturn.Exception == null)
                    {
                        var propertyName = input.MethodBase.Name.Substring(4);
                        (input.Target as INotifyPropertyChanged).RaisePropertyChangedEvent(this, new PropertyChangedEventArgs(propertyName, oldValue, newValue));
                    }
                }
            }
            else
            {
                methodReturn = getNext().Invoke(input, getNext);
            }

            return methodReturn;
        }

        private bool ShouldRaiseEvent(IMethodInvocation input, ref object oldValue, ref object newValue)
        {
            bool shouldRaiseEvent = false;
            var methodBase = input.MethodBase;
            var propertyName = methodBase.Name.Substring(4);
            var property = methodBase.ReflectedType.GetProperty(propertyName);

            var getMethod = property.GetGetMethod();
            if (getMethod != null)
            {
                var oldvalue = getMethod.Invoke(input.Target, null);
                var newvalue = input.Arguments[0];

                shouldRaiseEvent = newvalue == null ? newvalue != oldvalue : !newvalue.Equals(oldvalue);
                oldValue = oldvalue;
                newValue = newvalue;
            }

            return shouldRaiseEvent;
        }
    }

}
