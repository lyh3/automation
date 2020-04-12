using System;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public enum EnvironmentType { Production, UAT }

    public enum StatusType : int
    {
        InProcessing = 1,
        Submitted = 2,
        UnSubmitted = 3,
        Error = 4,
        ErrorPending = 5
    }

    public class GlobalDefinitions
    {
        public static readonly int MAX_PATH = 260;
        public static readonly string FtpPrefix = "ftp://";
        public static readonly string DefaultInfectedPassword = "infected";
        public static readonly string XceedZipLicenseKey = "SFN51-TC46X-EKGLY-Z83A";
    }
}
