using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    [TestFixture]
    public class ParserTest
    {
        private Parser<IEnumerable<string>> _helloWorldParser  = null;

        [SetUp]
        public void InitializeParser()
        {
            _helloWorldParser = MockHelloWorldParser();
        }

        [Test]
        public void ParseHelloWorld()
        {
            Action<IMaybe<Tuple<IEnumerable<string>, string>>> writeResult =
                s => Console.WriteLine(s.AsString(t => t.Aggregate("", (a, b) => a + " - " + b)));

            var r1 = _helloWorldParser("Hello World Hello World");
            writeResult(r1);//"- Hello - World - Hello - World"

            var r2 = _helloWorldParser("Hello      Hello Hello HelloWorld");
            writeResult(r2);//"- Hello - Hello - Hello - Hello - World"

            var r3 = _helloWorldParser("Hello x World");
            writeResult(r3);//"Nothing"
        }

        [Test]
        public void SimpleHelloWorldParser()
        {
            var helloWorldParser =
                from hello in "Hello".Find()
                from world in "World".Find()
                select new { Hello = hello, World = world };

            var result = helloWorldParser("HelloWorld");

            Console.WriteLine(result.AsString(x => x.Hello));
            Console.WriteLine(result.AsString(x => x.World));

            // outputs 
            // Hello
            // World
        }

        private Parser<IEnumerable<string>> MockHelloWorldParser()
        {
            return from token in "Hello".Find().Or("World".Find())
                   from _ in Parsers.WhiteSpace()
                   from list in Parsers.End(Enumerable.Empty<string>()).Or(MockHelloWorldParser())
                   select token.Cons(list);
        }
    }   
}
