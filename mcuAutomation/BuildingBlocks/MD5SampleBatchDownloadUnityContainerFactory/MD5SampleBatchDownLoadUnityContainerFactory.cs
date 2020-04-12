using System.Reflection;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;


using McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel;

namespace McAfeeLabs.Engineering.Automation.Base.MD5SampleBatchDownloadUnityContainerFactory
{
    public class MD5SampleBatchDownLoadUnityContainerFactory 
    {
        private static object _syncObj = new object();
        private static IUnityContainer _container;
        
        private MD5SampleBatchDownLoadUnityContainerFactory(){}

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
            XmlMergeUtil.MergeConfiguration(@"McAfeeLabs.Engineering.Automation.Base.MD5SampleBatchDownloadUnityContainerFactory", 
                                            @"XpathMerge.xml",
                                            Assembly.GetAssembly(typeof(MD5SampleBatchDownLoadUnityContainerFactory)),
                                            @".\Dynamic.config");
            _container = new UnityContainer();
            _container.LoadConfiguration();
        }
    }
}
