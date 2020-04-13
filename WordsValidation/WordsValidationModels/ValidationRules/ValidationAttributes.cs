using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsValidationModels
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    abstract public class ValidationAttributes : Attribute
    {
        protected ValidationRuleType _ruleType;
        public ValidationAttributes(ValidationRuleType ruleType)
        {
            _ruleType = ruleType;
        }
        public ValidationRuleType RuleType
        {
            get { return _ruleType; }
        }
    }

     public class LanguageValidationAttributes : ValidationAttributes
     {
         public LanguageValidationAttributes(ValidationRuleType ruleType):
             base(ruleType)
         {

         }
     }
}

