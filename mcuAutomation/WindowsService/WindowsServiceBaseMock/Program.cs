using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Windows.Service;

namespace WindowsServiceBaseMock {
    static class Program {              
            static void Main(string[] args) {
            var service = new WindowsServiceBaseMock(@"Mock McAfee Service", @"Mock McAfee Service");
            if (args.Length > 0 && !args[0].StartsWith("/")) {
                Console.WriteLine("Pleae run it with /d to debug, /i to install it as a service, or /u to uninstall it");
                Console.ReadLine();
                return;
            } else {
                ServiceEntryPoint entry = new ServiceEntryPoint(service);
                entry.EnterService(args);
            }            
        }
    }
}
