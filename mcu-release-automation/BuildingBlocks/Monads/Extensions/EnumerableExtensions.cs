using System;
using System.Collections.Generic;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }

        public static IEnumerable<B> Bind<A, B>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func)
        {
            foreach (var aval in a)
            {
                foreach (var bval in func(aval))
                {
                    yield return bval;
                }
            }
        }

        public static IEnumerable<C> SelectMany<A, B, C>(this IEnumerable<A> a, Func<A, IEnumerable<B>> func, Func<A, B, C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToEnumerable()));
        }
    }
}
