using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordsValidationModels
{
    abstract public class WordValidationRule : IValidationVisitor
    {
        protected Word _word = null;
        public WordValidationRule(Word word)
        {
            this._word = word;
        }

        abstract public bool Visit(Language language);
    }
}
