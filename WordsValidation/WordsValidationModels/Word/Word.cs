using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace WordsValidationModels
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IValidationVisitor
    {
        bool Visit(Language language);
    }

    abstract public class Word : ICommand
    {
        protected string _word = null;
        protected bool _isValid = true;

        public Word() { }
        public Word(string word) : this()
        {
            _word = word;
        }

        public bool IsValid { get { return _isValid; } }
        public string WordString { get { return _word; } }
        abstract public void Execute();

        /// <summary>
        /// This implementation uses Abstract Factory design pattern and leverages the .Net iflection class.  The same implementation
        /// can also be done in other program languages, such as Python
        /// 
        /// When a word string been passed in this factory method, the type of the concret object will be figured out automatically.
        /// An exception is thrown if the instance creation failed.
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        static public Word WordFactory(string word)
        {
            Word concretWordInstance = null;
            LanguageType? languageType = null;

            if (!string.IsNullOrEmpty(word))
            {
                foreach (var language in LanguageDictionary.Instance.LanguageData.Languages)
                {
                    var pattern = string.Format("[{0}]", string.Join(string.Empty, language.LanguageElements).ToLower());
                    var replaced = Regex.Replace(word.ToLower(), pattern, string.Empty);//This will trim all valid chars and leave the invalid chars.
                    if (Regex.Match(word.ToLower(), pattern).Success
                        && replaced.Length == 0)//the length == 0 means there is no illegal char in the input string against that language
                    {
                        languageType = (LanguageType)Enum.Parse(typeof(LanguageType), language.Name);
                        break;
                    }
                }
                var asm = Assembly.GetExecutingAssembly();
                var types = new List<Type>();
                types.AddRange(asm.GetTypes());
                if (null != asm && null != languageType)
                {
                    var className = string.Empty;
                    foreach (var type in types)
                    {
                        if (type.Name.StartsWith(languageType.ToString()))
                        {
                            concretWordInstance = Activator.CreateInstance(type, new object[] { word }) as Word;
                            break;
                        }
                    }

                }
            }

            if (null == concretWordInstance)
            {
                throw new ArgumentException(string.Format("The input word [{0}] is not in a supported lanuguage.", word));
            }

            concretWordInstance.Execute();

             return concretWordInstance;
        }

        protected void ApplyValidationRules(Language language)
        {
            var attrs = Attribute.GetCustomAttributes(this.GetType());
            if (null != attrs)
            {
                foreach (var attr in attrs)
                {
                    if (attr is LanguageValidationAttributes)
                    {
                        WordValidationRule rule = null;
                        var ruleType = ((LanguageValidationAttributes)attr).RuleType;
                        switch (ruleType)
                        {
                            case ValidationRuleType.BroadSlenderMutualExclusive:
                                rule = new BroadSlenderMutualExcludeRule(this);
                                break;
                            case ValidationRuleType.ConsonantsDuplicat:
                                rule = new ConsonantsDuplicatRule(this);
                                break;
                        }
                        if (null != rule)
                        {
                            this._isValid &= rule.Visit(language);
                        }
                    }
                }
            }
        }
    }
}
