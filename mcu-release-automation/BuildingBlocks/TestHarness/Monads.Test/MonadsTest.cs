using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Monads
{
    [TestFixture]
    public class MonadsTest
    {
        [Test]
        public void EnumerableComposition()
		{
            var sb = new StringBuilder();
			var aValues = Enumerable.Range(0, 2);
			var bValues = Enumerable.Range(3, 5);
			
			var results =
				aValues.Bind(a => 
				bValues.Bind(b => string.Format("[{0} {1}]", a, b).ToEnumerable()
					));

			foreach(var result in results)
                sb.Append(result);

            Assert.AreEqual(@"[0 3][0 4][0 5][0 6][0 7][1 3][1 4][1 5][1 6][1 7]", sb.ToString());
        }

        [Test]
        public void TaskComposition()
        {
            var aTask = Task.Factory.StartNew(() => Task1());
            var bTask = Task.Factory.StartNew(() => 4);

            var result =
                aTask.Bind(a =>
                bTask.Bind(b => //b.ToTask()
                    (a + b).ToTask()
                    ));

            result.ContinueWith( x => x);

            result.Wait(10000);
            Assert.AreEqual(result.Result, 7);
        }

        private int Task1()
        {
            return 3;
        }
    }   
}
