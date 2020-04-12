using System;
using System.Threading.Tasks;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    public static class TaskMonad
    {
        public static Task<T> ToTask<T>(this T value)
        {
            return Task.Factory.StartNew(() => value);
        }

        public static Task<B> Bind<A, B>(this Task<A> a, Func<A, Task<B>> func)
        {
            return a.ContinueWith(aTask => func(aTask.Result)).Unwrap();
        }

        public static Task<C> SelectMany<A, B, C>(this Task<A> a, Func<A, Task<B>> func, Func<A, B, C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToTask()));
        }
    }
}
