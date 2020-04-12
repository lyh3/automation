using System;
using System.Diagnostics;

using Microsoft.Practices.Unity;
using Avert.Automation.Support.DataAccessLayer.Factory;
using Avert.Automation.Support.DataAccessLayer.Handler;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class FileServiceMD5BatchDownload : MD5BatchDownload
    {
        #region Constructor

        [InjectionConstructor]
        public FileServiceMD5BatchDownload() { }

        #endregion
        
        #region Protected Methods

        override protected void MD5Download(string md5, string targetFolder, string fileExtension)
        {
            try
            {
                ChimeraFileOperationHandler datahandler = new ChimeraFileOperationHandler(Uri);
                datahandler.ChimeraFileDownload(@"md5",
                                                md5,
                                                string.IsNullOrEmpty(fileExtension)
                                                ? string.Format(@"{0}/{1}", targetFolder, md5)
                                                : string.Format(@"{0}/{1}.{2}", targetFolder, md5, fileExtension));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format(@"---Exception caught at MD5Download hash <{0}> from FileServiceMD5BatchDownload, targetFolder<{1}>, error was : {2}", md5, targetFolder, ex.Message));
                throw ex;
            }
        }

        #endregion
    }
}
