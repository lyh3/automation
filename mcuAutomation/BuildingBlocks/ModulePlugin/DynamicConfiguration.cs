using System;
using System.Linq;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace McAfeeLabs.Engineering.Automation.Profile.ModulePlugin
{
    public class DynamicConfiguration : DynamicPluginClient
    {
        #region Declarations

        public const string APPCONFIGFILE = @"APP_CONFIG_FILE";
        public const string InitState = @"s_initState";
        public const string ConfigSystem = @"s_configSystem";
        public const string Current = @"s_current";
        public const string AppSetttingsSection = @"appSettings";

        protected bool _canDispose;
        static protected string _path;
        private readonly string _originalConfig = AppDomain.CurrentDomain.GetData(APPCONFIGFILE).ToString();

        #endregion

        #region Constructors

        public DynamicConfiguration() { }

        #endregion

        #region Properties

        public bool CanDispose
        {
            get { return _canDispose; }
            set { _canDispose = value; }
        }

        public static string Path { get { return _path; } }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return "---- This is DynamicConfiguration.";
        }

        public void SwitchConfigSource(string path)
        {
            if (!File.Exists(path)) throw new ArgumentException(string.Format(@"Can not find the configuration file <{0}>", path));
            _path = path;
            AppDomain.CurrentDomain.SetData(APPCONFIGFILE, path);
            ResetConfigMechanism();
        }

        public override void Dispose()
        {
            if (!_canDispose)
                AppDomain.CurrentDomain.SetData(APPCONFIGFILE, _path);
            else
            {
                AppDomain.CurrentDomain.SetData(APPCONFIGFILE, _originalConfig);
                if (File.Exists(_path))
                    File.Delete(_path);
            }

            ResetConfigMechanism();
            GC.SuppressFinalize(this);
        }

        public void AppSectionAddVlue(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection(AppSetttingsSection);
        }

        public void AppSectionRemoveKey(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key])) return;

            config.AppSettings.Settings.Remove(key);
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection(AppSetttingsSection);
        }

        public void AddSection(string sectionName, ConfigurationSection section)
        {
            var map = new ExeConfigurationFileMap();
            map.LocalUserConfigFilename = _path;
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            config.Sections.Add(sectionName, section);
            config.Save(ConfigurationSaveMode.Modified, true);
        }

        #endregion

        #region Private Methods

        private static void ResetConfigMechanism()
        {
            typeof(ConfigurationManager)
                .GetField(InitState, BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, 0);

            typeof(ConfigurationManager)
                .GetField(ConfigSystem, BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, null);

            typeof(ConfigurationManager)
                .Assembly.GetTypes()
                .Where(x => x.FullName == "System.Configuration.ClientConfigPaths")
                .First()
                .GetField(Current, BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, null);
        }
        
        #endregion
    }

}
