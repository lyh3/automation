using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
namespace WordsValidationModels
{
    [Serializable]
    public class Language
    {
        private List<string> _broadVowelList = new List<string>();
        private List<string> _slenderVowelList = new List<string>();
        private List<string> _consonatList = new List<string>();

        public string Name { get; set; }
        public string [] BroadVowels
        {
            get { return _broadVowelList.ToArray(); }
            set
            {
                _broadVowelList.Clear();
                _broadVowelList.AddRange(value);
            }
        }
        public string[] SlenderVowels
        {
            get { return _slenderVowelList.ToArray(); }
            set
            {
                _slenderVowelList.Clear();
                _slenderVowelList.AddRange(value);
            }
        }
        public string[] Consonants
        {
            get { return _consonatList.ToArray(); }
            set
            {
                _consonatList.Clear();
                _consonatList.AddRange(value);
            }
        }
        /// <summary>
        /// The following four properties are applied the Compose Design Patterns
        /// </summary>
        [JsonIgnore]
        public string BroadVowelGroupPattern
        {
            get 
            {
                var s = string.Join(string.Empty, BroadVowels);
                return string.Format(Consts.GROUP_EXPRESSION_PATTERN_FORMAT, s.ToUpper(), s.ToLower());
            }
        }

        [JsonIgnore]
        public string SlenderVowelGroupPattern
        {
            get
            {
                var s = string.Join(string.Empty, SlenderVowels);
                return string.Format(Consts.GROUP_EXPRESSION_PATTERN_FORMAT, s.ToUpper(), s.ToLower());
            }
        }

        [JsonIgnore]
        public string ConsonantGroupPattern
        {
            get
            {
                var s = string.Join(string.Empty, Consonants);
                return string.Format(Consts.GROUP_EXPRESSION_PATTERN_FORMAT, s.ToUpper(), s.ToLower());
            }
        }
        /// <summary>
        /// This aggregated the entire string/chars defined in a language, inclu Broad Vowels, Slender Vowels and Consonants.
        /// The Aggregate Design Pattern is applied heere
        /// </summary>
        [JsonIgnore]
        public string[] LanguageElements 
        {
            get 
            {
                var aggregateData = new List<string>();
                aggregateData.AddRange(BroadVowels);
                aggregateData.AddRange(SlenderVowels);
                aggregateData.AddRange(Consonants);

                return aggregateData.ToArray();
            }
        }
    }

    [Serializable]
    public class LanguageData
    {
        private List<Language> _languageList = new List<Language>();

        public Language[] Languages
        {
            get { return _languageList.ToArray(); }
            set
            {
                _languageList.Clear();
                _languageList.AddRange(value);
            }
        }
        
        public override string ToString()
        {
            return new JsonFormatter.JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}
