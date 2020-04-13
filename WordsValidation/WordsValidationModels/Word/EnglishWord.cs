//#define USE_DEFAULT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordsValidationModels
{
    [LanguageValidationAttributes(ValidationRuleType.ConsonantsDuplicat)]
    [LanguageValidationAttributes(ValidationRuleType.BroadSlenderMutualExclusive)]
    public class EnglishWord : Word
    {
        public EnglishWord(string word):base(word)
        {
        }

        public override void Execute()
        {
           base.ApplyValidationRules(LanguageDictionary.Instance[LanguageType.English]);
        }
    }
}
