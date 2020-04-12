using System;
using System.ComponentModel;
using System.Reflection;

namespace  McAfeeLabs.Engineering.Automation.Base
{
    public class RunWorkerCompletedEventArgsWithUserState : RunWorkerCompletedEventArgs
    {
        public RunWorkerCompletedEventArgsWithUserState(object result, Exception error, bool cancelled, object userState)
            : base(result, error, cancelled)
        {
            Type[] ctorArgs = { typeof(Exception), typeof(bool), typeof(object) };
            ConstructorInfo constructorInfo = typeof(AsyncCompletedEventArgs).GetConstructor(ctorArgs);

            constructorInfo.Invoke(this, new object[] { error, cancelled, userState });
        }

    }
}
