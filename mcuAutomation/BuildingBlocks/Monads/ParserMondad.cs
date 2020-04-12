using System;
using System.Collections.Generic;
using System.Linq;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    public delegate IMaybe<Tuple<T, string>> Parser<T>(string input);

    public static class Parsers
    {
        public static Parser<string> WhiteSpace()
        {
            return s => new Just<Tuple<string, string>>(Tuple.Create<string, string>(null, s.TrimStart()));
        }

        public static Parser<string> Find(this string stringToFind)
        {
            return s => s.StartsWith(stringToFind)
                ? new Just<Tuple<string, string>>(Tuple.Create(stringToFind, s.Skip(stringToFind.Length).AsString()))
                : (IMaybe<Tuple<string, string>>)new Nothing<Tuple<string, string>>();
        }

        public static Parser<T> End<T>(T initialValue)
        {
            return s => (s == "")
                ? new Just<Tuple<T, string>>(Tuple.Create(initialValue, s))
                : (IMaybe<Tuple<T, string>>)new Nothing<Tuple<T, string>>();
        }

        public static string AsString(this IEnumerable<char> chars)
        {
            return new string(chars.ToArray());
        }

        public static string AsString<T>(this IMaybe<Tuple<T, string>> parseResult, Func<T, string> unwrap)
        {
            var justParseResult = parseResult as Just<Tuple<T, string>>;

            return (justParseResult != null)
                       ? unwrap(justParseResult.Value.Item1)
                       : "Nothing";
        }
    }
}
