using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    [TestFixture]
    public class FuntionCompositionTest
    {
        [Test]
        public void ComposingSimpleFunctions()
        {
            // two simple functions that take an int and return an int
            Func<int, int> add2 = x => x + 2;
            Func<int, int> mult2 = x => x * 2;

            var a = 5;

            // we can execute them immediately, one after another
            var r1 = mult2(add2(a));
            Console.Out.WriteLine("r1 = {0}", r1);
            Assert.AreEqual(r1, 14);

            // or we can 'compose' them
            Func<int, int> add2Mult2 = x => mult2(add2(x));

            // and then use the new function later
            var r2 = add2Mult2(a);
            Console.Out.WriteLine("r2 = {0}", r2);
            Assert.AreEqual(r2, 14);
        }

        [Test]
        public void ComposingFunctionsWithAmplifiedValues()
        {
            // two simple functions that take an int and return an Identity<int>
            Func<int, Identity<int>> add2 = x => new Identity<int>(x + 2);
            Func<int, Identity<int>> mult2 = x => new Identity<int>(x * 2);

            var a = 5;

            // we can't compose them directly, the types don't match. This won't compile:
            // Func<int, Identity<int>> add2Mult2 = x => mult2(add2(x).Value);

            // we need a 'Bind' function to compose them:
            Func<int, Identity<int>> add2Mult2 = x => add2(x).Bind(mult2);

            // we can now use add2Mult2 at some later date
            var r1 = add2Mult2(a);
            Console.Out.WriteLine("r1.Value = {0}", r1.Value);
            Assert.AreEqual(r1.Value, 14);
        }

        [Test]
        public void WriteArbitraryIdentityExpressions()
        {
            var result =
                "Hello World!".ToIdentity().Bind(a =>
                7.ToIdentity().Bind(b =>
                (new DateTime(2010, 1, 11)).ToIdentity().Bind(c =>
                (a + ", " + b.ToString() + ", " + c.ToShortDateString())
                .ToIdentity())));

            Console.WriteLine(result.Value);
            Assert.AreEqual(result.Value, @"Hello World!, 7, 1/11/2010");
        }

        [Test]
        public void WriteArbitaryIdentityExpressionsWithLinq()
        {
            var result =
                from a in "Hello World!".ToIdentity()
                from b in 7.ToIdentity()
                from c in (new DateTime(2010, 1, 11)).ToIdentity()
                select a + ", " + b.ToString() + ", " + c.ToShortDateString();

            Console.WriteLine(result.Value);
            Assert.AreEqual(result.Value, @"Hello World!, 7, 1/11/2010");
        }

        [Test]
        public void AddTwoNumbers()
        {
            var result =
                from a in 3.ToIdentity()
                from b in 4.ToIdentity()
                select a + b;

            Console.Out.WriteLine("result.Value = {0}", result.Value);
            Assert.AreEqual(result.Value, 7);
        }
    }   
}
