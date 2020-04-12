namespace WindowService.DataModel
{
    public class Definitions
    {
        public const int MAX_ONE_TIME_MCU_RELEASE = 10;
        public const string REQUIRED_FIELD_MESSAGE = @"--- CAUTION: This is a required field, please enter values !";
        public const string OPTIONAL_FIELD_MESSAGE = @"The value for the field will be extracted from header information.";
        public const string HEALTH_EVENT_JSON_FILE = @"HelathEvent.json";//@"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\HelathEvent.json";
        public const string SPS_TOOLS_MODEL_KEY = @"SpsToolsViewModel";
        public const string HEALTH_EVENT_MODEL_KEY = @"HealthEventViewModel";
        public const string SYSTEM_RESET_MODEL_KEY = @"SystemRemoteResetViewModel";
        public const string RESET_WORK_THREAD_KEY = @"SystemRemoteResetWorkthread";
        public const string COMMAND_MAP_KEY = @"IpmiCommandMapKey";
        public const string BIOS_IMAGE_OPERATIONS_KEY = @"BiosImageOperationsViewModel";
        public const string BKC_IMAGE_PARSER_KEY = @"BkcParserViewModel";
        public const string RELEASE_ORCHSTRATION_KEY = @"ReleaseOrchstration";
        public const string MICROCODE_RELEASE_ORCHSTRATION_KEY = @"MicrocodeReleaseOrchstration";
        public const string MCU_DEFAULT_FORMAT = "{0}_{1}";
        public const string GENERIC_MCU_BINARY_NAME = "GenericBinaryName";
        public const string MCU_VIEW_DATA_KEY = "McuViewDataKey";
        public const string VIEW_DATA_NOTIFICATION_SP_KEY = "ViewDataNotificationSp";
        public const string VIEW_DATA_RELEASE_TO_KEY = "ViewDataReleaseTo";
        public const string README_WATCH_KEY = "ReadmeFileWatcher";
        public const string HSDMAP_WATCH_KEY = "HsdMapFileWatcher";
        public const string RELEASE_TO_SESSION_KEY = "ReleaseToSessionKey";
    }

    public enum SpsToolsType
    {
        HealthEventDecode,
        SpiImageMerge,
        MeFWStatusDecode
    }
    public enum SpiMergeFileType
    {
        SourceImage,
        RegionImage,
        MapFile
    }
    public enum BiosOperationFileType
    {
        IfwiImage,
        BiosImage,
        OutpotFile
    }
    public enum BiosImageOperationType
    {
        ExtractBios,
        MergeBios,
        SwapBios
    }
    public enum DeviceType
    {
        BMC,
        Aadvark
    }
    public enum CommandType
    {
        Warm,
        AcCycle,
        MeCold,
        CustomerDefined
    }
    public enum MeMode
    {
        Recovery,
        Operational
    }
    public enum BiosOperationViewStatus
    {
        ConfigEdit,
        Operation
    }
    public enum IfwiBkcParserViewType
    {
        UserInputConfig,
        EmbeddedConfig,
    }
    public enum SupportedPlatform
    {
        Whitley,
        Mehlow,
        Purley,
        Backerville,
        Jacobsville,
        Willsonville
    }
    public enum ReleaseSource
    {
        HSD,
        Local
    }
    public enum CpuSegment
    {
        Desktop,
        Server,
        Mobile,
        SOC,
        MAX_CPU_SEGMENT
    }
    public enum McuTableColumn
    {
        CpuSegment,
        McuFileName,
        CpuCodeName,
        CpuCoreStepping,
        PlatformId,
        CpuId,
        CpuPublicSpecUpdate,
        IntelProductSpecs,
        CpuNDASpecUpdate,
        ProcessorModel,
        Products
    }
}
