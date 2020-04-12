using System.Text;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy
{
    public class SmartSMSBatchDownload : MD5BatchDownload
    {
        #region Constructor

        [InjectionConstructor]
        public SmartSMSBatchDownload() { }

        #endregion
        
        #region Protected Methods

        override protected void MD5Download(string md5, string targetFolder, string fileExtension)
        {
            md5.SmartSMSDownload(targetFolder, fileExtension, _errorMessages);
        }

        #endregion
    }
}
