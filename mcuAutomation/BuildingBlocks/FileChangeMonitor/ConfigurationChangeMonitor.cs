using System;
using System.IO;
using System.Xml;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class ConfigurationChangeMonitor : FileChangeMonitor
    {
        #region Declaration

        private event EventHandler _eventConfigurationChanged;

        #endregion

        #region Constructor
        public ConfigurationChangeMonitor() { }

        public ConfigurationChangeMonitor(string filepath)
            : this(filepath, @"*.config")
        {
        }

        public ConfigurationChangeMonitor(string filepath, string filter)
            : base(filepath, filter)
        {
        }

        #endregion

        #region Properties

        public event EventHandler ConfigurationChanged
        {
            add { _eventConfigurationChanged = (EventHandler)Delegate.Combine(_eventConfigurationChanged, value); }
            remove { _eventConfigurationChanged = (EventHandler)Delegate.Remove(_eventConfigurationChanged, value); }
        }

        #endregion

        #region Protected Methods

        protected override void NotifyChange(FileSystemEventArgs e)
        {
            if (null != _eventConfigurationChanged)
                _eventConfigurationChanged(this, new ConfigurationChangedEventArgs(e));
        }

        #endregion
    }

    public class ConfigurationChangedEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public XmlDocument ConfigXml { get; set; }
        public ConfigurationChangedEventArgs(FileSystemEventArgs e)
        {
            FilePath = e.FullPath;
            ConfigXml = new XmlDocument();
            ConfigXml.Load(FilePath);
        }
    }
}
