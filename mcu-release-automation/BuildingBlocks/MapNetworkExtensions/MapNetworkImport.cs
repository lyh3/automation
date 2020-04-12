using System;
using System.Runtime.InteropServices;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public partial class MapNetworkDriveHelper
    {
        #region Consts

        public const int RESOURCE_CONNECTED = 0x00000001;
        public const int RESOURCE_GLOBALNET = 0x00000002;
        public const int RESOURCE_REMEMBERED = 0x00000003;

        public const int RESOURCETYPE_ANY = 0x00000000;
        public const int RESOURCETYPE_DISK = 0x00000001;
        public const int RESOURCETYPE_PRINT = 0x00000002;

        public const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        public const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        public const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        public const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        public const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        public const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

        public const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        public const int RESOURCEUSAGE_CONTAINER = 0x00000002;


        public const int CONNECT_INTERACTIVE = 0x00000008;
        public const int CONNECT_PROMPT = 0x00000010;
        public const int CONNECT_REDIRECT = 0x00000080;
        public const int CONNECT_UPDATE_PROFILE = 0x00000001;
        public const int CONNECT_COMMANDLINE = 0x00000800;
        public const int CONNECT_CMD_SAVECRED = 0x00001000;

        public const int CONNECT_LOCALDRIVE = 0x00000100;

        #endregion

        #region Errors Code
        public const int NO_ERROR = 0;

        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_ALREADY_ASSIGNED = 85;
        public const int ERROR_BAD_DEVICE = 1200;
        public const int ERROR_BAD_NET_NAME = 67;
        public const int ERROR_BAD_PROVIDER = 1204;
        public const int ERROR_CANCELLED = 1223;
        public const int ERROR_EXTENDED_ERROR = 1208;
        public const int ERROR_INVALID_ADDRESS = 487;
        public const int ERROR_INVALID_PARAMETER = 87;
        public const int ERROR_INVALID_PASSWORD = 1216;
        public const int ERROR_MORE_DATA = 234;
        public const int ERROR_NO_MORE_ITEMS = 259;
        public const int ERROR_NO_NET_OR_BAD_PATH = 1203;
        public const int ERROR_NO_NETWORK = 1222;

        public const int ERROR_BAD_PROFILE = 1206;
        public const int ERROR_CANNOT_OPEN_PROFILE = 1205;
        public const int ERROR_DEVICE_IN_USE = 2404;
        public const int ERROR_NOT_CONNECTED = 2250;
        public const int ERROR_OPEN_FILES = 2401;

        private struct ErrorWrapper
        {
            public int num;
            public string message;
            public ErrorWrapper(int num, string message)
            {
                this.num = num;
                this.message = message;
            }
        }

        private static ErrorWrapper[] ERROR_LIST = new ErrorWrapper[] 
        {
            new ErrorWrapper(ERROR_ACCESS_DENIED, "Error: Access Denied"), 
            new ErrorWrapper(ERROR_ALREADY_ASSIGNED, "Error: Already Assigned"), 
            new ErrorWrapper(ERROR_BAD_DEVICE, "Error: Bad Device"), 
            new ErrorWrapper(ERROR_BAD_NET_NAME, "Error: Bad Net Name"), 
            new ErrorWrapper(ERROR_BAD_PROVIDER, "Error: Bad Provider"), 
            new ErrorWrapper(ERROR_CANCELLED, "Error: Cancelled"), 
            new ErrorWrapper(ERROR_EXTENDED_ERROR, "Error: Extended Error"), 
            new ErrorWrapper(ERROR_INVALID_ADDRESS, "Error: Invalid Address"), 
            new ErrorWrapper(ERROR_INVALID_PARAMETER, "Error: Invalid Parameter"), 
            new ErrorWrapper(ERROR_INVALID_PASSWORD, "Error: Invalid Password"), 
            new ErrorWrapper(ERROR_MORE_DATA, "Error: More Data"), 
            new ErrorWrapper(ERROR_NO_MORE_ITEMS, "Error: No More Items"), 
            new ErrorWrapper(ERROR_NO_NET_OR_BAD_PATH, "Error: No Net Or Bad Path"), 
            new ErrorWrapper(ERROR_NO_NETWORK, "Error: No Network"), 
            new ErrorWrapper(ERROR_BAD_PROFILE, "Error: Bad Profile"), 
            new ErrorWrapper(ERROR_CANNOT_OPEN_PROFILE, "Error: Cannot Open Profile"), 
            new ErrorWrapper(ERROR_DEVICE_IN_USE, "Error: Device In Use"), 
            new ErrorWrapper(ERROR_EXTENDED_ERROR, "Error: Extended Error"), 
            new ErrorWrapper(ERROR_NOT_CONNECTED, "Error: Not Connected"), 
            new ErrorWrapper(ERROR_OPEN_FILES, "Error: Open Files"), 
        };

        public static string ParseError(int errNum)
        {
            foreach (ErrorWrapper er in ERROR_LIST)
            {
                if (er.num == errNum) return er.message;
            }
            return string.Format(@"The error <{0}> can not be parsed.", errNum);
        }

        #endregion

        [DllImport("Mpr.dll", EntryPoint = "WNetUseConnection", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NetResource lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
            );

        [DllImport("Mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int WNetCancelConnection2(
            string lpName,
            int dwFlags,
            bool fForce
            );

        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int WNetAddConnection2A(ref NetResource netresource,
            string password,
            string username,
            int flags);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = string.Empty;
            public string lpRemoteName = string.Empty;
            public string lpComment = string.Empty;
            public string lpProvider = string.Empty;
        }
    }
}

