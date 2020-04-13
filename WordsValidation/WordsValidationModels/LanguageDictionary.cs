using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WordsValidationModels
{
    public class LanguageDictionary
    {
        const string JSON_FILE = @"..\..\..\WordsValidation.json";
        private static object _syncObj = new object();
        private static LanguageDictionary _instance = null;
        private static LanguageData _languageData = null;

        private LanguageDictionary()
        {
            using (StreamReader file = File.OpenText(JSON_FILE))
            {
                try
                {
                    var serializer = new JsonSerializer();
                    _languageData = (LanguageData)serializer.Deserialize(file, typeof(LanguageData));
                }
                catch (Exception ex)
                {
                }
            }
        }

        #region Indexer
        public Language this[LanguageType languageType]
        {
            get { return _languageData.Languages.FirstOrDefault<Language>(x => x.Name == languageType.ToString()); }
        }
        #endregion

        public static LanguageDictionary Instance
        {
            get
            {
                lock (_syncObj)
                {
                    if (null == _instance)
                    {
                        _instance = new LanguageDictionary();
                    }
                }
                return _instance;
            }
        }
    }
}
