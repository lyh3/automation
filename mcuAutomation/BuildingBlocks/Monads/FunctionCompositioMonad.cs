using System;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    public static class FunctionCompositioMonad
    {
        // These two functions make Identity<T> a Monad.

        // a function 'Unit' or 'Return' or 'ToIdentity' that creates a new instance of Identity
        public static Identity<T> ToIdentity<T>(this T value)
        {
            return new Identity<T>(value);
        }

        // a function 'Bind', that allows us to compose Identity returning functions
        public static Identity<B> Bind<A, B>(this Identity<A> a, Func<A, Identity<B>> func)
        {
            return func(a.Value);
        }

        public static Identity<C> SelectMany<A, B, C>(this Identity<A> a, Func<A, Identity<B>> func, Func<A, B, C> select)
        {
            return select(a.Value, func(a.Value).Value).ToIdentity();
        }
    }
}
