using Microsoft.Practices.Unity;
using Xceed.Zip;
using Ionic.Zip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class XcessdZipProvider : IZipProvider
    {
        private string _sourceArchiveFile;

        #region Constructor

        [InjectionConstructor]
        public XcessdZipProvider()
        {
            Xceed.Zip.Licenser.LicenseKey = GlobalDefinitions.XceedZipLicenseKey;
        }

        #endregion

        public string SourcArchive
        {
            get { return _sourceArchiveFile; }
            set{_sourceArchiveFile = value;}
        }

        public bool? ThrowException { get; set; }

        #region Properties

        public bool IsArchive { get { return string.IsNullOrEmpty(_sourceArchiveFile) ? false : ZipFile.IsZipFile(_sourceArchiveFile); } }

        #endregion

        #region Public Methods

        public void ExtractArchiveTo(string distinationFolder, string password = null)
        {
            QuickZip.Unzip(_sourceArchiveFile, distinationFolder, password, true, true, true, "*");
        }

        #endregion
    }
}
