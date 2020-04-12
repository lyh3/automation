using System;
using System.IO;
using System.Reflection;
using System.Xml;

using log4net;

using McAfeeLabs.Engineering.Automation.Profile;
using McAfeeLabs.Engineering.Automation.Profile.ConfigProvider;
using McAfeeLabs.Engineering.Automation.Profile.ModulePlugin;
using McAfeeLabs.Engineering.Automation.Base.XmlMergeDataModel;

namespace McAfee.Service
{
    public abstract partial class McAfeeServiceThreadModel
    {
        #region Declarations

        private string _modulename;
        private string _profileName;
        private Assembly _entryAssembly;
        private string _defaultPluginsDomainFolder;

        #endregion

        #region Protected Methods

        protected void UnloadPlugin()
        {
            ModulePluginLoader.UnLoadModulePlugin();
        }

        protected void ReloadModulePlugin()
        {
            if (string.IsNullOrEmpty(_modulename)
            || string.IsNullOrEmpty(_profileName)
            || string.IsNullOrEmpty(_defaultPluginsDomainFolder)
            || null == _entryAssembly)
                return;

            try
            {
                InitializeModuleDynamicConfig();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(@"---Exception caught at ReloadPlugin, error was: {0}", ex.ToString());
            }
        }

        #endregion
        
        #region Private Methods

        void InitializePlugins()
        {
            _modulename = this.GetType().Module.Name;
            _profileName = string.Format(@"{0}.config", _modulename);
            _modulename = _modulename.Replace(Path.GetExtension(_modulename), string.Empty);
            _entryAssembly = Assembly.GetEntryAssembly();
            _defaultPluginsDomainFolder = string.Format(@"{0}{1}",
                                          AppDomain.CurrentDomain.BaseDirectory,
                                          PluginManager.DefaultDomainName);
            ReloadModulePlugin();
        }

        protected void InitializeModuleDynamicConfig()
        {
            const string dynamicConfigFileName = @".\Dyanmic.config";
            var serviceNamespace = this.GetType().Namespace;
            string mergeScriptName = @"DefaultPlugins.XpathMergeRelease.xml";
            #if DEBUG
            mergeScriptName = @"DefaultPlugins.XpathMergeDebug.xml";
            #endif
            var dynamicconfig = new DynamicConfiguration();
            var executingAssembly = Assembly.GetExecutingAssembly();
            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginManager.DefaultDomainName);

            ModulePluginLoader.LoadDefaultModulePlugins(pluginFolder,
                                                        new SQLConfigProvider(executingAssembly));
            
            //Merge the profile database connection in order to query configuration from DB
            XmlMergeUtil.MergeConfiguration(serviceNamespace,
                                            mergeScriptName,
                                            executingAssembly,
                                            dynamicConfigFileName,
                                            _logger);

            var profile = _modulename.QueryProfile(AppTypeEnum.exe, _profileName, ProfileTypeEnum.config);
            File.Delete(dynamicConfigFileName);

            if (null == profile || string.IsNullOrEmpty(profile.XmlPayload))
            {
                //Switch back to original configuration if failed to query configuration from DB
                //If query the centralized configuration failed for any reason.
                var currentConfigPath = string.Format(@"{0}{1}.exe.config", AppDomain.CurrentDomain.BaseDirectory, _entryAssembly.GetName().Name);
                dynamicconfig.SwitchConfigSource(currentConfigPath);
                ModulePluginLoader.InitializeLogger(out _logger, this);
                Directory.Delete(pluginFolder, true);
                return;
            }

            //hook up configuration update notification when the updating occurs from DB, the loacal configuration will sync automatically. Then this call is 
            //done the current configuration will sync the the configuration retrieved from the Db and automatically synchronized if there are updated from Db.
            ModulePluginLoader.LoadModulePlugin(_defaultPluginsDomainFolder,
                                                new SQLConfigProvider(_entryAssembly)
                                                {
                                                    ProfileFileName = _profileName,
                                                    UpdateCheckInterval = 10 * 1000,
                                                    TargetPath = @".\"
                                                },
                                                this);
        }

        #endregion
    }
}
