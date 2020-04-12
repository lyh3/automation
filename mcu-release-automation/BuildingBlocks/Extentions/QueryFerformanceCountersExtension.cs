using System;
using System.Diagnostics;
using System.Threading;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public static class QueryFerformanceCountersExtension
    {
        public static float CPUCounters(this string serverName)
        {
            return QueryPerformanceCounter(serverName,
                                           @"Processor",
                                           @"% Processor time",
                                           @"_Total");
        }

        public static float CommittedMemoryCounters(this string serverName)
        {
            return QueryPerformanceCounter(serverName,
                                           @"Memory",
                                           @"% Committed Bytes In Use");
        }

        public static float AvailableMBytesMemoryCounters(this string serverName)
        {
            return QueryPerformanceCounter(serverName,
                                           @"Memory",
                                           @"Available MBytes");
        }

        public static float QueryPerformanceCounter( string serverName, 
                                                     string category, 
                                                     string counterName, 
                                                     string instanceName = "")
        {
            float val = (float)100.0;
            try
            {
                var performanceCounter = new PerformanceCounter(category,
                                                                counterName,
                                                                instanceName,
                                                                serverName);

                performanceCounter.NextValue();
                Thread.Sleep(500);// 500 ms wait
                val = performanceCounter.NextValue();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format(@"--- Exception caught at QueryPerformanceCounterValue, error was:{0}", ex.Message));
            }

            return val;
        }
    }
}
