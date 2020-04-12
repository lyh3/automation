using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Configuration.Install;

using log4net;

namespace Windows.Service {
    public abstract partial class WindowsService : ServiceBase {
        #region Declarations
        protected static ILog _logger;
        protected string _serviceName = string.Empty;
        private string _serviceDescription = string.Empty;
        private ServiceStartMode _startType = ServiceStartMode.Automatic;
        #endregion

        #region Constructor
        public WindowsService(string serviceName, string serviceDescription, ServiceStartMode startType = ServiceStartMode.Automatic) {
            this.ServiceName = serviceName;            
            this.EventLog.Log = "Windows";
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            _serviceName = serviceName;
            _serviceDescription = serviceDescription;
            _startType = startType;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(this.GetType());
        }
        #endregion

        #region Properties
        public ILog Logger { get { return _logger; } }
        public string ServiceDescription { get { return _serviceDescription; } }
        public ServiceStartMode StartType { get { return _startType; } }
        #endregion

        #region Public Methods

        public abstract void StartService();
        public abstract void StopService();
                
        public bool InstallService() {
            string processPath = Application.ExecutablePath;

            try {
                TransactedInstaller tranInstaller = new TransactedInstaller();
                WindowsServiceInstaller serviceInstaller = new WindowsServiceInstaller(_serviceName, _serviceDescription, _startType);

                tranInstaller.Installers.Add(serviceInstaller);
                string[] cmd = { "/assemblypath=" + processPath };
                InstallContext context = new InstallContext(null, cmd);
                tranInstaller.Context = context;

                //delete the source if it's already there, installation failes if the source is already there
                if (EventLog.SourceExists(_serviceName))
                    EventLog.DeleteEventSource(_serviceName);

                tranInstaller.Install(new Hashtable());
            } catch (Exception e) {
                _logger.Error("Install failed for service " + _serviceName, e);
                return false;
            }

            return true;
        }

        public bool UninstallService() {
            string processPath = Application.ExecutablePath;

            try {
                TransactedInstaller tranInstaller = new TransactedInstaller();
                WindowsServiceInstaller serviceInstaller = new WindowsServiceInstaller(_serviceName, _serviceDescription, _startType);

                tranInstaller.Installers.Add(serviceInstaller);
                string[] cmd = { "/assemblypath=" + processPath };
                InstallContext context = new InstallContext(null, cmd);
                tranInstaller.Context = context;

                tranInstaller.Uninstall(null);
            } catch (Exception e) {
                _logger.Error("Uninstall failed for service : " + _serviceName, e);
                return false;
            }

            return true;
        }
        
        #endregion

        #region Protected Methods

        protected override void OnStart(string[] args) {
            StartService();

            base.OnStart(args);
        }

        protected override void OnStop() {
            StopService();

            base.OnStop();
        }

        protected void InitializeLogger(string namedLogger) {
            _logger = LogManager.GetLogger(string.IsNullOrEmpty(namedLogger) ? @"Default" : namedLogger);
            log4net.Config.XmlConfigurator.Configure();
        }

        protected virtual void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            _logger.Error("Unhandled Exception", e.ExceptionObject as Exception);
            Environment.Exit(0);
        }

        #endregion
    }
}
