#define USE_CONFIGURATION
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class UniversalUnityContainerFactory
    {
        private static object _syncObj = new object();
        private static IUnityContainer _container;

        private UniversalUnityContainerFactory() { }

        public static IUnityContainer Instance
        {
            get
            {
                lock (_syncObj)
                {
                    if (null == _container) InitializeContainer();
                    return _container;
                }
            }
        }

        private static void InitializeContainer()
        {
            _container = new UnityContainer();
            #if USE_CONFIGURATION
            _container.LoadConfiguration();
            #else
            _container.RegisterType<IMD5BatchDowload, FileServiceMD5BatchDownload>(new ContainerControlledLifetimeManager())//new ConainerControlledLifetimeManager() for Singleton
                      ;
            _container.AddNewExtension<Interception>()
            .Configure<Interception>()
            .SetInterceptorFor<IMD5BatchDowload>(new InterfaceInterceptor());
            #endif
        }
    }
}
