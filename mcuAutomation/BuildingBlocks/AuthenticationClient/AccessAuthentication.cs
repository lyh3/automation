using System;

namespace McAfee.Automation.AuthenticationClient
{
    public class AccessAuthentication
    {
        #region Declarations

        private static AccessAuthentication _instance = null;
        private static Object syncObj = new object();

        #endregion

        #region Constructor

        private AccessAuthentication() 
        {
            Authentication = new WCFAuthenticationProxy.AuthenticationService.AuthenticationClient();
        }

        #endregion

        #region Properties

        public static AccessAuthentication Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new AccessAuthentication();
                 }

                return _instance;
            }
        }

        public WCFAuthenticationProxy.AuthenticationService.AuthenticationClient Authentication { get; set; }

        #endregion
    }
}
