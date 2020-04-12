using System.Reflection;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    public partial class FTPFile
    {
        static public IUnityContainer UnityContainerFactory()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IPerformanceCounterService>(new PerformanceCounterService { Category = FTPFile.PerfofmanceCounterCategory })
                     .RegisterInstance<IFTPFile>(new FTPFile());
            container.AddNewExtension<Interception>()
            .Configure<Interception>()
            .SetInterceptorFor<IFTPFile>(new InterfaceInterceptor());

            PerformanceCounterInstall.CreateCounters(typeof(FTPFile), Assembly.GetAssembly(typeof(FTPClient)), FTPFile.PerfofmanceCounterCategory);

            return container;
        }
    }
}
