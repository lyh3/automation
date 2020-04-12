using System.Reflection;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    public partial class FTPFolder
    {
        static public IUnityContainer UnityContainerFactory()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IPerformanceCounterService>(new PerformanceCounterService { Category = FTPFolder.PerfofmanceCounterCategory })
                     .RegisterInstance<IFTPFolder>(new FTPFolder());
            container.AddNewExtension<Interception>()
            .Configure<Interception>()
            .SetInterceptorFor<IFTPFolder>(new InterfaceInterceptor());

            PerformanceCounterInstall.CreateCounters(typeof(FTPFolder), Assembly.GetAssembly(typeof(FTPClient)), FTPFile.PerfofmanceCounterCategory);

            return container;
        }
    }
}
