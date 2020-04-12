#define USE_INTERFACE
//#define USE_CONFIGURATION
//#define USE_OBJ_PROXY

//#define USE_WCF

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.Configuration;
using log4net;
//using AOP.Library.Authorization.WcfAuthorizationService;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    //using BuildingBlocks.Shared;

    abstract public class TestBase
    {
        public const int NumberOfThread = 1000;
        private ILog _log = LogManager.GetLogger(typeof(TestBase));
        protected IUnityContainer container;

#if USE_INTERFACE
        protected ITestData TestData;
#endif
#if USE_CONFIGURATION
        protected ITestData TestData;
#endif
#if USE_OBJ_PROXY
        protected TestData TestData;
#endif

        public TestBase()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeContainer();
        }

        private void InitializeContainer()
        {
            container = new UnityContainer();
            var categories = new List<string>();

#if USE_INTERFACE
            container.RegisterType<ITestData, TestData>(new ContainerControlledLifetimeManager())//new ConainerControlledLifetimeManager() for Singleton
                     .RegisterInstance<IPerformanceCounterService>(new PerformanceCounterService { Category = PerformanceCounterInstall.DefaultPerformanceCounterCategory })
                     .RegisterInstance(_log);
                     container.AddNewExtension<Interception>()
                     .Configure<Interception>()
                     .SetInterceptorFor<ITestData>(new InterfaceInterceptor());
            //container.LoadConfiguration();
#endif
#if USE_CONFIGURATION
            container.LoadConfiguration();
#endif
#if USE_OBJ_PROXY
            container.AddNewExtension<Interception>();
            container.RegisterType<TestData>(
                    new InterceptionBehavior<PolicyInjectionBehavior>(),
                    new Interceptor<TransparentProxyInterceptor>())
                    .RegisterType<IPerformanceCounterService, PerformanceCounterService>()
#if USE_WCF
                    .RegisterType<IAuthorizationService, AuthorizationServiceClient>()
#else
                    .RegisterType<IAuthorizationService, AccessAuthorizationTestProvider>()
#endif
                    .RegisterInstance(_log);
#endif
        }

        protected void InitializeTestDataInstance()
        {
#if USE_INTERFACE
            TestData = container.Resolve<ITestData>();
#endif
#if USE_CONFIGURATION
            TestData = container.Resolve<ITestData>();
#endif
#if USE_OBJ_PROXY
            TestData = container.Resolve<TestData>();
#endif
        }
    }
}
