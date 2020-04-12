using System;
using System.Collections.Generic;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Profile.ModulePlugin
{
    public enum AppTypeEnum { exe, dll, wcf, web, MAX_AppTypeEnum }
    public enum ProfileTypeEnum { config, license, icon, javascript, readme }

    public interface IModulePlugin : IDisposable
    {
        event EventHandler PluginModuleCallback;
    }
}
