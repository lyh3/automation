using System;
using System.Security.Permissions;

using McAfeeLabs.Engineering.Automation.Base.FileLogListener;

namespace McAfeeLabs.Engineering.Automation.Profile.ModulePlugin
{
    abstract public class DynamicPluginClient : MarshalByRefObject, IModulePlugin
    {
        #region Declarations

        protected event EventHandler _eveluginModuleCallback;

        #endregion

        #region Constructor

        public DynamicPluginClient()
        {
            FileLogListener.AddFileLogListener(this);
        }

        #endregion

        #region Properties

        public event EventHandler PluginModuleCallback
        {
            add { _eveluginModuleCallback = (EventHandler)Delegate.Combine(_eveluginModuleCallback, value); }
            remove { _eveluginModuleCallback = (EventHandler)Delegate.Remove(_eveluginModuleCallback, value); }
        }

        #endregion

        #region Public Methods

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        abstract public void Dispose();
    }
}
