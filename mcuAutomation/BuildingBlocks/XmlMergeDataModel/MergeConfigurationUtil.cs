using System;
using System.Xml;
using System.Reflection;

using log4net;
using McAfeeLabs.Engineering.Automation.Profile.ModulePlugin;

namespace McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel
{
    public class XmlMergeUtil
    {
        public static void MergeConfiguration(string resourceNamespace, 
                                              string mergeXmlFileName, 
                                              Assembly containerAssembly,
                                              string dynamicConfigFileName,
                                              ILog logger = null)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var configPath = string.Format(@"{0}{1}.exe.config", AppDomain.CurrentDomain.BaseDirectory, entryAssembly.GetName().Name);
            var targetXmlDoc = CommonUtility.LoadAppConfigXml(configPath);

            var mergeScriptXmlDoc = new XmlDocument();
            var embeddedXmlFile = string.Format(@"{0}.{1}", resourceNamespace, mergeXmlFileName);
            embeddedXmlFile.LoadEmbeddedXml(mergeScriptXmlDoc, containerAssembly);

            Merge(dynamicConfigFileName, logger, targetXmlDoc, mergeScriptXmlDoc);
        }

        public static void Merge ( string dynamicConfigFileName, 
                                   ILog logger, 
                                   XmlDocument targetXmlDoc,
                                   XmlDocument mergeScriptXmlDoc)
        {
            if(null == targetXmlDoc || null == mergeScriptXmlDoc) return;

            try
            {
                var mergeworker = new MergeWorker(logger, mergeScriptXmlDoc, targetXmlDoc);
                mergeworker.Merge();

                //var dynamicConfigFileName = @".\Dynamic.config";
                mergeworker.MergedXmlDoc.Save(dynamicConfigFileName);
                using (var dynamicconfig = new DynamicConfiguration())
                {
                    dynamicconfig.SwitchConfigSource(dynamicConfigFileName);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(@"---Exception caught at XmlMergeUtil, error was:{0}", ex);
                if (null != logger)
                    logger.Error(errorMessage, ex);
                else
                    System.Diagnostics.Trace.WriteLine(errorMessage);
            }
        }
    }
}
