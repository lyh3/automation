// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class IniFile
    {
        private static Regex settingPattern = new Regex(@"^\s*(?<Key>[^=:]+)[=:](?<Value>.*?)(?<Comment>(?<!\\)[#;].*)*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        private static Regex sectionPattern = new Regex(@"^\s*\[(?<Name>[^\[\]]+)\]\s*([#;].*)?$", RegexOptions.Compiled);
        private static Regex emptyLinePattern = new Regex(@"^\s*([#;].*)?$", RegexOptions.Compiled);

        /// <summary>
        /// Creates a new, empty, INI file.
        /// </summary>
        public IniFile()
        {
            Sections = new IniSections();
            Parameters = new IniParameters();
            FilePath = "";
        }

        /// <summary>
        /// Creates a new IniFile that is loaded from a path.
        /// </summary>
        /// <param name="filePath">Loads from Path</param>
        public IniFile(string filePath)
            : this()
        {
            using (StreamReader rdr = new StreamReader(filePath))
            {
                IniParameters currentSection = Parameters;

                String line;
                int lineNum = 0;
                while ((line = rdr.ReadLine()) != null)
                {
                    lineNum++;

                    //if it is a setting it is added to the current section
                    //if it is a section the current section is changed and added to the file
                    //otherwise, if the line is not blank or commented an error is thrown
                    Match settingMatch = settingPattern.Match(line);
                    if (settingMatch.Success)
                    {
                        //keys are stored as uppercase
                        string key = settingMatch.Groups["Key"].Value.Trim();
                        string value = settingMatch.Groups["Value"].Value.Trim();

                        //Uncommand and save the following if you wish to record comments
                        // string comment = settingMatch.Groups[""].Value.Trim();

                        currentSection.Add(key, Unescape(value));
                        continue;
                    }

                    Match sectionMatch = sectionPattern.Match(line);
                    if (sectionMatch.Success)
                    {
                        IniSection section = new IniSection(sectionMatch.Groups["Name"].Value);
                        currentSection = section.Parameters;
                        Sections.Add(section);
                        continue;
                    }

                    if (!emptyLinePattern.IsMatch(line))
                        throw new InvalidDataException("Input file contains incorrect formatting on line " + lineNum.ToString());
                }
            }

            FilePath = filePath;
        }

        /// <summary>
        /// The full path and name of the ini file.
        /// Example: 
        /// "C:\folder\file.ini"
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// A dictionary of sections within the INI file.
        /// </summary>
        public IniSections Sections { get; set; }

        /// <summary>
        /// A dictionary of parameters not contained within a specific section.
        /// </summary>
        public IniParameters Parameters { get; set; }

        /// <summary>
        /// Attempts to save the INI file record to its associated physical file.
        /// </summary>
        public void SaveFile()
        {
            if (String.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("This INI record has no associated file.");
            SaveFile(FilePath);
        }

        /// <summary>
        /// Attempts to save the INI file record to the specified physical 
        /// file. It is created if it does not exist.
        /// </summary>
        /// <param name="filePath">
        /// The full path to the file which will be created or overwritten.
        /// </param>
        public void SaveFile(string filePath, bool escape = true)
        {
            using (StreamWriter w = new StreamWriter(filePath, false, Encoding.Default))
            {

                //the first written parameters are the ones without a heading
                foreach (KeyValuePair<string, string> parameter in this.Parameters)
                {
                    w.WriteLine("{0}={1}", parameter.Key, Escape(parameter.Value));
                }
                bool firstSection = true;
                //there may be multiple sections, so loop through all of them
                foreach (IniSection section in this.Sections.Values)
                {
                    if (!firstSection)
                        w.WriteLine();

                    firstSection = false;
                    w.WriteLine("[{0}]", section.SectionName);

                    //most INI conventions state that keys are case insensitive, and this
                    //will convert them to uppercase for clarity.
                    foreach (KeyValuePair<string, string> parameter in section.Parameters)
                    {
                        w.WriteLine("{0}={1}", parameter.Key, escape ? Escape(parameter.Value) : parameter.Value);
                    }
                }
            }
        }

        //a pattern gifted unto thee by the regex gods
        //        private const string INI_SETTING_PATTERN = @"
        //					^\s*                        #accounts for leading spaces
        //					(?<Key>                     #start of key group
        //						[A-Za-z0-9_-]+          #key is made up of letters, digits and _ or -
        //					)                           
        //					\s*                         #accounts for spaces before delimiter                
        //					[=:]                        #delimiter may be a : or =
        //					\s*                         #spaces after delimiter
        //					(?<Value>                   #value group
        //						[^\s#;:=]+?             #at least one non-whitespace or comment character, lazy
        //						((?<=\\)[=:]|[^=:])*?   #doesnt match unescaped = or :, is lazy to exclude whitespace and comments
        //					)                           
        //					\s*                         #account for spaces before comment or after value
        //					((?<!\\)[#;].*)?            #a comment starting with # or ;
        //					$                           #end of string";



        /// <summary>
        /// Gets or sets the setting with name key in the specified section.
        /// If it does not exist setting it will create it and getting it will return null.
        /// </summary>
        /// <param name="sectionName">name of section to place key in.</param>
        /// <param name="key">the unique key name linked to this setting</param>
        /// <returns>value associated with key or null if it does not exist</returns>
        public string this[string sectionName, string key]
        {
            get
            {
                if (Sections.ContainsKey(sectionName) &&
                    Sections[sectionName].Parameters.ContainsKey(key))
                    return Sections[sectionName].Parameters[key];
                else
                    return null;
            }
            set
            {
                if (Sections.ContainsKey(sectionName))
                {
                    IniParameters p = Sections[sectionName].Parameters;
                    if (p.ContainsKey(key))
                        p[key] = value;
                    else
                        p.Add(key, value);
                }
                else
                {
                    IniSection section = new IniSection(sectionName);
                    section.Parameters.Add(key, value);
                    Sections.Add(section);
                }
            }
        }

        /// <summary>
        /// Gets or sets the setting with name key in the INI files uncategorized paramaters.
        /// </summary>
        /// <param name="key">the unique key name linked to this setting</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (Parameters.ContainsKey(key))
                    return Parameters[key];
                else
                    return null;
            }
            set
            {
                if (Parameters.ContainsKey(key))
                    Parameters[key] = value;
                else
                    Parameters.Add(key, value);
            }
        }

        /// <summary>
        /// Escapes a string so it is a legal value inside a .INI file.
        /// </summary>
        /// <param name="value">The string to escape</param>
        /// <returns>Escaped String</returns>
        private static string Escape(string value)
        {
            string pattern = @"[#;:\\=\t\r\n]";
            MatchEvaluator eval = (MatchEvaluator)delegate(Match m)
            {
                string s = m.Value;
                if (s == "\r")
                    return "\\r";
                else if (s == "\n")
                    return "\\n";
                else if (s == "\t")
                    return "\\t";
                else
                    return "\\" + s;
            };

            return Regex.Replace(value, pattern, eval);

        }

        /// <summary>
        /// Unescapes a string that was escaped inside a .INI file
        /// </summary>
        /// <param name="value">The string to Unescape</param>
        /// <returns>Unescaped String</returns>
        private static string Unescape(string value)
        {
            string pattern = @"\\([#;:\\=trn])";
            MatchEvaluator eval = (MatchEvaluator)delegate(Match m)
            {
                string s = m.Groups[1].Value;
                if (s == "r")
                    return "\r";
                else if (s == "n")
                    return "\n";
                else if (s == "t")
                    return "\t";
                else
                    return s;
            };

            return Regex.Replace(value, pattern, eval);

        }

    }


    //-------------Classes that are used with the INI file--------------//
    //just a section with a name and parameters
    public class IniSection
    {
        public IniParameters Parameters { get; set; }
        public string SectionName { get; set; }

        public IniSection(string sectionName)
        {
            SectionName = sectionName;
            Parameters = new IniParameters();
        }
    }

    //customized dictionary with uppercase key
    public class IniSections : Dictionary<string, IniSection>
    {
        public void Add(IniSection section)
        {
            base.Add(section.SectionName, section);
        }

        new public void Add(string name, IniSection section)
        {
            if (!section.SectionName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentOutOfRangeException("Name must be same as section name");
            else
                Add(section);
        }

        new public bool ContainsKey(string key)
        {
            return base.ContainsKey(key);
        }

        new public IniSection this[string key]
        {

            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }

    //basically a string dictionary with uppercase keys
    public class IniParameters : Dictionary<string, string>
    {
        new public void Add(string key, string value)
        {
            base.Add(key, value);
        }

        new public bool ContainsKey(string key)
        {
            return base.ContainsKey(key);
        }

        new public string this[string key]
        {

            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
