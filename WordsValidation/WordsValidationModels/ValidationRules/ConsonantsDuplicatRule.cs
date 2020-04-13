using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordsValidationModels
{

    public class ConsonantsDuplicatRule : WordValidationRule
    {
        public ConsonantsDuplicatRule(Word word)
            : base(word)
        {
        }

        public override bool Visit(Language language)
        {
            var validationTemplate = this._word.WordString;
            foreach (var vowelsPattern in new[] { language.BroadVowelGroupPattern, language.SlenderVowelGroupPattern })
            {
                validationTemplate = Regex.Replace(validationTemplate, vowelsPattern, string.Empty);
            }
            var hasDuplicate = Regex.Match(validationTemplate.ToLower(), @"(.)\1+").Success;
            return !hasDuplicate;
        }
    }

}
