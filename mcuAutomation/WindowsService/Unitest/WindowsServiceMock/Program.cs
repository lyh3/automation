using System;

namespace McAfee.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new WindowsServiceMock(@"Mock McAfee Service", "Mock Service for Unitest");
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
