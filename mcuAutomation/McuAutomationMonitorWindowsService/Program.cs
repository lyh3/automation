using System;
using log4net;
using Windows.Service;

namespace McuAutomationMonitorWindowsService
{
    static class Program
    {
        static ILog _logger;
        static void Main(string[] args)
        {
            const string serviceName = @"MCU release automation monitor";
            _logger = LogManager.GetLogger(@"Default");
            log4net.Config.XmlConfigurator.Configure();

            var service = new McuAutomationMonitorService(serviceName, @"Mcu release automation serive monitor", _logger);

            if (args.Length > 0 && !args[0].StartsWith("/"))
            {
                Console.WriteLine("Pleae run it with /d to debug, /i to install it as a service, or /u to uninstall it");
                Console.ReadLine();
                return;
            }
            else
            {
                ServiceEntryPoint entry = new ServiceEntryPoint(service);
                entry.EnterService(args);
            }
        }
     }
}
