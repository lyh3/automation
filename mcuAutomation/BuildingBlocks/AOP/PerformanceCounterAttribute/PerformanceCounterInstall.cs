using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public class PerformanceCounterInstall
    {
        public const string DefaultPerformanceCounterCategory = "AOP Performance Counters --- Buildingblocks";
        private const string WindowsInstallEnvironmentVariableName = "windir";

        #region Public Methods

        public static void CreateCounters(Type type, Assembly assembly, string counterCategory = null)
        {
            var category = string.IsNullOrEmpty(counterCategory) ? DefaultPerformanceCounterCategory : counterCategory;
            try
            {
                if (!PerformanceCounterCategory.Exists(category))
                {
                    var counters = new CounterCreationDataCollection();
                    foreach (var counter in ReflectConters(type))
                    {
                        counters.Add(CreateCounter(counter.ToString(), category));
                        counters.Add(CreateCounter(counter + "Duration", category));
                        counters.Add(CreateCounter(counter + "Base", category));
                    }
                    PerformanceCounterCategory.Create(category,
                                                      "This section is a grouping of counters for the AOP Performance Counters Service",
                                                      PerformanceCounterCategoryType.SingleInstance, counters);
                    RegisterComponent(true, category, assembly);
                }
            }
            catch{}
        }

        public static void RemovePerformanceCounterCategory(string counterCategory, Assembly assembly = null )
        {
            try
            {
                var category = counterCategory;
                if (string.IsNullOrEmpty(category))
                    category = DefaultPerformanceCounterCategory;

                if (PerformanceCounterCategory.Exists(category))
                {
                    RegisterComponent(false, counterCategory, assembly );
                    PerformanceCounterCategory.Delete(category);
                }
            }
            catch(Exception ex)
            {
            }
        }

        #endregion

        #region Private Methods

        private static CounterCreationData CreateCounter(string name, string help)
        {
            return new CounterCreationData
            {
                CounterName = name,
                CounterHelp = help,
                CounterType = name.Contains("Base") ? PerformanceCounterType.AverageBase
                    : name.Contains("Duration") ? PerformanceCounterType.AverageTimer32
                    : PerformanceCounterType.RateOfCountsPerSecond32
            };
        }

        private static IEnumerable ReflectConters(Type type)
        {
            List<string> counters = new List<string>();

            ReflectCountrAttributes(type, ref counters);

            foreach (var member in type.GetMembers())
                ReflectCountrAttributes(member, ref counters);

            return counters;
        }

        private static void ReflectCountrAttributes(dynamic source, ref List<string> counterNames)
        {
            IEnumerator attr = source.GetCustomAttributes(true).GetEnumerator();
            while (attr.MoveNext())
            {
                var counter = attr.Current is IncrementCounterAttribute ? (attr.Current as IncrementCounterAttribute).Increment
                    : attr.Current is IncrementCounterAttribute ? (attr.Current as DurationCounterAttribute).Clock : null;
                if (!string.IsNullOrEmpty(counter) && !counterNames.Contains(counter))
                    counterNames.Add(counter);
            }
        }

        public static void RegisterComponent(bool install, string counterCategory, Assembly assembly = null)
        {
            try
            {
                string installUtilPath = string.Format(@"{0}\Microsoft.NET\Framework\v2.0.50727\installutil.exe",
                                                       Environment.GetEnvironmentVariable(WindowsInstallEnvironmentVariableName));
                Assembly asm = null == assembly ? Assembly.GetExecutingAssembly() : assembly;
                if (null != asm)
                {
                    string argFormat = string.Format("/category=\"{0}\" {1}",
                                                    counterCategory,
                                                    "{0}.dll");
                    if (install == false)
                    {
                        argFormat += " /u";
                    }

                    Process installUtilProc = new Process();
                    installUtilProc.StartInfo.FileName = installUtilPath;
                    installUtilProc.StartInfo.Arguments = string.Format(argFormat, asm.FullName.Substring(0, asm.FullName.IndexOf(',')));
                    installUtilProc.Start();
                }
            }
            catch {}
        }

        #endregion    
    }
}
