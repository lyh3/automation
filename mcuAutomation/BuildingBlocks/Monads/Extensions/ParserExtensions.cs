using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    public static class ParserExtensions
    {
        public static Parser<T> ToParser<T>(this T value)
        {
            return s => new Just<Tuple<T, string>>(Tuple.Create(value, s));
        }

        public static Parser<B> Bind<A, B>(this Parser<A> a, Func<A, Parser<B>> func)
        {
            return s =>
            {
                var aMaybe = a(s);
                var aResult = aMaybe as Just<Tuple<A, string>>;

                // short circuit if parse fails
                if (aResult == null) return new Nothing<Tuple<B, string>>();

                var aValue = aResult.Value.Item1;
                var sString = aResult.Value.Item2;

                var bParser = func(aValue);
                return bParser(sString);
            };
        }

        public static Parser<C> SelectMany<A, B, C>(this Parser<A> a, Func<A, Parser<B>> func, Func<A, B, C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToParser()));
        }

        public static Parser<T> Or<T>(this Parser<T> a, Parser<T> b)
        {
            return s =>
            {
                var aMaybe = a(s);
                return aMaybe is Just<Tuple<T, string>> ? aMaybe : b(s);
            };
        }
    }
}
