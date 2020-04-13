using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordsValidationModels
{

    public class BroadSlenderMutualExcludeRule : WordValidationRule
    {
        public BroadSlenderMutualExcludeRule(Word word)
            : base(word)
        {
        }
        public override bool Visit(Language language)
        {
            var validationTemplate = Regex.Replace(this._word.WordString, language.ConsonantGroupPattern, "*");//}ComposeVowlsPattern(language);
            var split = (from x in validationTemplate.Split('*')
                         where !string.IsNullOrEmpty(x)
                         select x).ToArray();
            var isInSameGroup = true;
            var i = 0;
            var j = 1;
            for (; j < split.Length; ++i, ++j)
            {
                isInSameGroup &= VowelsGroupCheck(split[i], split[j], language);
            }
            return isInSameGroup;
        }
        private bool VowelsGroupCheck(string leftHandOfConsonantGroup,
                                string righHandOfConsonantGrop,
                                Language language)
        {
            var _isInTheSameVoweGroup = false;
            var broadVowelsPattern = language.BroadVowelGroupPattern;
            var slenderVowelsPattern = language.SlenderVowelGroupPattern;

            _isInTheSameVoweGroup = (Regex.Match(leftHandOfConsonantGroup, broadVowelsPattern).Success
                                     && !Regex.Match(righHandOfConsonantGrop, slenderVowelsPattern).Success)
                                  ^ (!Regex.Match(leftHandOfConsonantGroup, broadVowelsPattern).Success
                                     && Regex.Match(righHandOfConsonantGrop, slenderVowelsPattern).Success);
            return _isInTheSameVoweGroup;
        }
    }

}
