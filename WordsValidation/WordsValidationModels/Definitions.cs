using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsValidationModels
{
    public class Consts
    {
        public const string GROUP_EXPRESSION_PATTERN_FORMAT = @"([{0}{1}]+)";
    }

    public enum LanguageType
    {
        English,
        Alias,
        International,
        Russian
    }

    public enum ValidationRuleType
    {
        BroadSlenderMutualExclusive,
        ConsonantsDuplicat
    }
}
