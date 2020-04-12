//#define USE_CODE_BEHIND
using System;
using System.IO;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.Configuration;

using Xceed.Zip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class ZipProviderContainer
    {
        private static ZipProviderContainer _instance;
        private static object _syncObj = new object();
        private static UnityContainer _container;

        private ZipProviderContainer()
        {
            IntialContainer();
        }

        public static ZipProviderContainer Instance
        {
            get
            {
                lock (_syncObj)
                {
                    if (null == _instance)
                        _instance = new ZipProviderContainer();
                    return _instance;
                }
            }
        }

        public IZipProvider ZipProvider { get { return _container.Resolve<IZipProvider>(); } }

        private static void IntialContainer()
        {
            _container = new UnityContainer();

            #if USE_CODE_BEHIND
            _container.RegisterType<IZipProvider, SevenZipSharpWrapper>(new ContainerControlledLifetimeManager())//new ConainerControlledLifetimeManager() for Singleton
                      ;
            _container.AddNewExtension<Interception>()
            .Configure<Interception>()
            .SetInterceptorFor<IZipProvider>(new InterfaceInterceptor());
            #else
            var unityConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ZipWalker.dll.config");
            var unityConfig = unityConfigPath.GetExternalConfiguration();
            var unitySection = (UnityConfigurationSection)unityConfig.GetSection(@"unity");
            _container.LoadConfiguration(unitySection);
            #endif
            var zipProvider = _container.Resolve<IZipProvider>();
            Xceed.Zip.Licenser.LicenseKey = GlobalDefinitions.XceedZipLicenseKey;
        }
    }
}
