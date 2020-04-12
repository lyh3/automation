using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;        //for RunInstaller decoration
using System.ServiceProcess;
using System.Configuration.Install;

namespace Windows.Service
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        public WindowsServiceInstaller(string serviceName, string serviceDescription, ServiceStartMode startType = ServiceStartMode.Automatic)
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.DisplayName = serviceName;
            serviceInstaller.Description = serviceDescription;
            serviceInstaller.StartType = startType;
            //serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = serviceName;

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
