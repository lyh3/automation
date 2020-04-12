namespace McAfeeLabs.Engineering.Automation.Base
{
    public interface IZipProvider
    {
        bool IsArchive { get; }
        string SourcArchive { get; set; }
        bool? ThrowException { get; set; }
        void ExtractArchiveTo( string distinationFolder, string password = null);
    }
}
