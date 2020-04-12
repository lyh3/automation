using System;
using System.Collections.Generic;
using System.ServiceProcess;                                                                             
using System.Runtime.InteropServices;

namespace Windows.Service {
    public class ServiceEntryPoint {
        #region DllImport

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        #endregion

        #region Declarations

        private WindowsService _service = null;

        private string _serviceName = null;
        private string _serviceDescription = null;

        #endregion

        public ServiceEntryPoint(WindowsService service) {
            _service = service;
            _serviceName = _service.ServiceName;
            _serviceDescription = _service.ServiceDescription;
        }

        public void EnterService(string[] args) {
            bool useConsole = false;

            if (args.Length > 0)
            {
                string __firstArg = args[0];
                if (string.IsNullOrEmpty(__firstArg))
                {
                    throw new Exception("Command line arguement not valid");
                }

                if (__firstArg.ToLower() == "/d")
                {
                    useConsole = true;
                }
                else if (__firstArg.ToLower() == "/i")
                {
                    AllocConsole();
                    _service.InstallService();
                    Environment.Exit(1);
                }
                else if (__firstArg.ToLower() == "/u")
                {
                    AllocConsole();
                    _service.UninstallService();
                    Environment.Exit(1);
                }
            }

             Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (useConsole) {
                AllocConsole();
                Console.WriteLine("To debug, attach now and press a key to start.  Otherwise, just hit a key to start");
                Console.ReadKey();
                _service.StartService();
                DebugInConsole(_service);
            } else {
                ServiceBase.Run(_service);
            }
        }

        public static void DebugInConsole(WindowsService service) {
            Console.WriteLine("Debugging begun");
            while (true) {
                Console.WriteLine("Type 'stop' to stop debugging : ");
                string command = Console.ReadLine();
                if (command.ToLower() == "stop") {
                    service.StopService();
                    break;
                }
            }
            Environment.Exit(1);
        }
    }
}
