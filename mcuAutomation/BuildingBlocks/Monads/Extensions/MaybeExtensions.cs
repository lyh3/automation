using System;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    public static class MaybeExtensions
    {
        public static IMaybe<T> ToMaybe<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static IMaybe<B> Bind<A, B>(this IMaybe<A> a, Func<A, IMaybe<B>> func)
        {
            var justa = a as Just<A>;
            return justa == null ? new Nothing<B>() 
                                 :func(justa.Value);
        }

        public static IMaybe<C> SelectMany<A, B, C>(this IMaybe<A> a, Func<A, IMaybe<B>> func, Func<A, B, C> select)
        {
            return a.Bind(aval =>
                    func(aval).Bind(bval =>
                    select(aval, bval).ToMaybe()));
        }

        public static IMaybe<B> Select<A, B>(this IMaybe<A> a, Func<A, IMaybe<B>> func)
        {
            return a.Bind(func);
        }
    }
}
