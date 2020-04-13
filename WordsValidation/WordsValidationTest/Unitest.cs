using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

using NUnit.Framework;
using NUnit.Compatibility;
using Newtonsoft.Json;
using WordsValidationModels;

namespace WordsValidationTest
{
    [TestFixture]
    public class Unitest
    {
        [SetUp]
        public void Initialization()
        {
            var instance = LanguageDictionary.Instance;
            instance.LanguageData = null;
        }

        [Test]
        public void LanguageDictionaryTest()
        {
            var instance = LanguageDictionary.Instance;
            Assert.AreNotEqual(string.Empty, instance.LanguageData.ToString());
            var english = instance[LanguageType.English];
            Assert.AreNotEqual(null, english);
            Assert.AreNotEqual(string.Empty, english.ToString());
            Assert.AreEqual(LanguageType.English.ToString(), english.Name);
        }

        [Test]
        public void LanguageDictionaryExceptionTest()
        {
            const string JSON_FILE = @"..\..\..\WordsValidation.json";
            var tempFileName = string.Format("{0}_Temp.json", JSON_FILE.Replace(".json", string.Empty));
            try
            {
                File.Move(JSON_FILE, tempFileName);
                var instance = LanguageDictionary.Instance;
                Assert.AreNotEqual(null, instance);
                Assert.AreEqual(null, instance.LanguageData);
            }
            finally
            {
                File.Move(tempFileName, JSON_FILE);
            }
        }

        [Test]
        public void SingletonTest()
        {
            var instane1 = LanguageDictionary.Instance;
            var instane2 = LanguageDictionary.Instance;
            Assert.AreSame(instane1, instane2);
        }

        [Test]
        public void EnglishWordValidationSuccessTest()
        {
            const string word = "Automaton";

            var wordInstance = Word.WordFactory(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(EnglishWord), wordInstance.GetType());

            Assert.AreEqual(true, wordInstance.IsValid);
        }

        [Test]
        public void EnglishWordVowlsRuleValidationFailureTest()
        {
            const string word = "Automation";

            var wordInstance = Word.WordFactory(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(EnglishWord), wordInstance.GetType());

            Assert.AreEqual(false, wordInstance.IsValid);
        }

        [Test]
        public void EnglishWordConsonatsViolationRuleFailureTest()
        {
            const string word = "AutomatXXYYZZon";

            var wordInstance = Word.WordFactory(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(EnglishWord), wordInstance.GetType());

            Assert.AreEqual(false, wordInstance.IsValid);
        }

        [Test]
        public void EnglishWordBothVowelsRuleAndConsonatsViolationFailureTest()
        {
            const string word = "AutomatXXYYZZon";

            var wordInstance = Word.WordFactory(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(EnglishWord), wordInstance.GetType());

            Assert.AreEqual(false, wordInstance.IsValid);
        }
        [Test]
        public void RussianhWordValidationSuccessTest()
        {
            const string word = "рАбОмАчЪ";

            var wordInstance = Word.WordFactory(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(RussianWord), wordInstance.GetType());

            Assert.AreEqual(false, wordInstance.IsValid);
        }

        [Test]
        public void AliasWordTest()
        {
            const string word = "AutomatXXYYZZion";

            var wordInstance = new AliasWord(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(AliasWord), wordInstance.GetType());
            wordInstance.Execute();
            Assert.AreEqual(true, wordInstance.IsValid);
        }

        [Test]
        public void InternationalWordTest()
        {
            const string word = "AutomatXXYYZZion";

            var wordInstance = new InternationalWord(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(InternationalWord), wordInstance.GetType());
            wordInstance.Execute();
            Assert.AreEqual(true, wordInstance.IsValid);
        }

        [Test]
        public void WordFactorySuccessTest()
        {
            const string word = "Automation";
            var wordInstance = Word.WordFactory(word);
            Assert.AreNotEqual(null, wordInstance);
            Assert.AreEqual(typeof(EnglishWord), wordInstance.GetType());
        }

        [Test]
        public void WordFactoryFailureWithMixedEnglishAndRussianTest()
        {
            const string word = "Automatщionф";
            Exception expectedException = null;
            try
            {
                var wordInstance = Word.WordFactory(word);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }
            Assert.AreNotEqual(null, expectedException);
            Assert.AreEqual(typeof(ArgumentException), expectedException.GetType());
        }

        [Test]
        public void WordFactoryFailureWithMixedEnglishAndChinesTest()
        {
            const string word = "中文Automation中文";
            Exception expectedException = null;
            try
            {
                var wordInstance = Word.WordFactory(word);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }
            Assert.AreNotEqual(null, expectedException);
            Assert.AreEqual(typeof(ArgumentException), expectedException.GetType());
        }
    }

}
