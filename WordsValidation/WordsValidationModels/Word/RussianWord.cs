using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsValidationModels
{
    [LanguageValidationAttributes(ValidationRuleType.BroadSlenderMutualExclusive)]
    public class RussianWord : Word
    {
        public RussianWord(string word):base(word)
        {
        }

        public override void Execute()
        {
            base.ApplyValidationRules(LanguageDictionary.Instance[LanguageType.Russian]);
        }
    }
}
