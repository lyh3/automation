
using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    [TestFixture]
    public class ValidationTest : TestBase
    {
        public ValidationTest():base() 
        {
            InitializeTest();
        }

        [SetUp]
        public void InitializeTest()
        {
            InitializeTestDataInstance();
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestIntBelowRange()
        {
            var val = -1;
            TestData.IntVal = val;
            Assert.AreNotEqual(TestData.IntVal, val);
        }

        [Test]
        public void TestIntInRange()
        {
            var val = 10;
            TestData.IntVal = val;
            Assert.AreEqual(TestData.IntVal, val);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestIntBeyondRangeException()
        {
            int val = 9999;
            TestData.IntVal = val;
            Assert.AreNotEqual(TestData.IntVal, val);
        }

        [Test]
        public void TestGoodRegex()
        {
            var s = @"foo@gmail.com";
            TestData.Email = s;
            Assert.AreEqual(TestData.Email, s);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestBadRegexException()
        {
            var s = @"foo";
            TestData.Email = s;
            Assert.AreNotEqual(TestData.Email, s);
        }

        [Test]
        public void TestStringLength()
        {
            TestData.String = "Hello";
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestAboveMaxLength()
        {
            TestData.String = "somereallylongname@somecompanythatistoolongtofitin30characters.com";
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestBelowMinLength()
        {
            TestData.String = string.Empty;
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestFloatBelowRange()
        {
            var val = -1f;
            TestData.Float = val;
            Assert.AreNotEqual(TestData.Float, val);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestFloatAboveRange()
        {
            var val = 21f;
            TestData.Float = val;
            Assert.AreNotEqual(TestData.Float, val);
        }

        [Test]
        public void TestFloatInRange()
        {
            var val = 10f;
            TestData.Float = val;
            Assert.AreEqual(TestData.Float, val);
        }
    }

}
