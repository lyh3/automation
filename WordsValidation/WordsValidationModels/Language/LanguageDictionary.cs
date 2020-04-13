using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace WordsValidationModels
{
    /// <summary>
    /// This class is implemented unsing Sington Design Pattern
    /// </summary>
    public class LanguageDictionary
    {
        const string JSON_FILE = @"..\..\..\WordsValidation.json";
        private static object _syncObj = new object();
        private static LanguageDictionary _instance = null;
        private static LanguageData _languageData = null;

        /// <summary>
        /// Private constuctor that is required by Sington pattern
        /// </summary>
        private LanguageDictionary()
        {
        }

        #region Indexer
        public Language this[LanguageType languageType]
        {
            get { return _languageData.Languages.FirstOrDefault<Language>(x => x.Name == languageType.ToString()); }
        }
        #endregion

        /// <summary>
        /// Thread safe implementation
        /// </summary>
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
                    if (null == _languageData)
                    {
                        try
                        {
                            using (StreamReader file = File.OpenText(JSON_FILE))
                            {
                                var serializer = new JsonSerializer();
                                _languageData = (LanguageData)serializer.Deserialize(file, typeof(LanguageData));
                            }
                        }
                        catch { }
                    }
                }
                return _instance;
            }
        }
        public LanguageData LanguageData
        {
            get { return _languageData; }
            set { _languageData = value; }
        }
    }
}
