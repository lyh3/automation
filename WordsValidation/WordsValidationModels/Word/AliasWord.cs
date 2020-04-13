using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsValidationModels
{
    public class AliasWord : Word
    {
        public AliasWord(string word): base(word)
        {
        }

        public override void Execute()
        {
            base.ApplyValidationRules(LanguageDictionary.Instance[LanguageType.Alias]);
        }
    }
}
