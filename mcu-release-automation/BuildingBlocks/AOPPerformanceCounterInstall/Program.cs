using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using log4net;
using McAfeeLabs.Engineering.Automation.Base.FTPClient;
using McAfeeLabs.Engineering.Automation.AOP;
using Automation.RaidenCollectionSubmission.Component;

namespace AOPPerformanceCounterInstall
{
    class Program
    {
        static private ILog _logger;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(typeof(Program));
            var install = true;
            if (args.Length > 0)
                install = args[0] == "/i" ? true : false;

            try
            {
                if (install)
                {
                    PerformanceCounterInstall.CreateCounters(typeof(FTPFolder), Assembly.GetAssembly(typeof(FTPClient)), FTPFile.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.CreateCounters(typeof(FTPFile), Assembly.GetAssembly(typeof(FTPClient)), FTPFile.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.CreateCounters(typeof(SSCHandler), Assembly.GetAssembly(typeof(SSCHandler)), SSCHandler.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.CreateCounters(typeof(FileServiceRunner), Assembly.GetAssembly(typeof(FileServiceRunner)), FileServiceRunner.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.CreateCounters(typeof(HashServiceRunner), Assembly.GetAssembly(typeof(HashServiceRunner)), HashServiceRunner.PerfofmanceCounterCategory);
                    _logger.Info("--- Install AOP performance counters success.");
                }
                else
                {
                    PerformanceCounterInstall.RemovePerformanceCounterCategory(FTPFile.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.RemovePerformanceCounterCategory(FTPFile.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.RemovePerformanceCounterCategory(SSCHandler.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.RemovePerformanceCounterCategory(FileServiceRunner.PerfofmanceCounterCategory);
                    PerformanceCounterInstall.RemovePerformanceCounterCategory(HashServiceRunner.PerfofmanceCounterCategory);
                    _logger.Info("--- Remove AOP performance counters success.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format(@"--- Exception caught to install performance counter, error was: {0}", ex.Message));
            }

            Console.ReadLine();
        }
    }
}
