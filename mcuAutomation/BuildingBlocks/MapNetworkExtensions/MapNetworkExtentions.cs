using System;

namespace McAfeeLabs.Engineering.Automation.Base
{
    static public class MapNetworkDriveExtentions
    {
        public static string ConnectToRemote(this string remotePath, 
                                            string username, 
                                            string password,
                                            string localPath = null)
        {
            return ConnectToRemote(remotePath, username, password, false);
        }

        private static string ConnectToRemote( string remotePath, 
                                               string username, 
                                               string password, 
                                               bool promptUser)
        {
            MapNetworkDriveHelper.NetResource nr = new MapNetworkDriveHelper.NetResource();
            nr.dwType = MapNetworkDriveHelper.RESOURCETYPE_DISK;
            nr.lpRemoteName = remotePath;

            int ret;
            if (promptUser)
                ret = MapNetworkDriveHelper.WNetUseConnection(IntPtr.Zero, 
                    nr, 
                    string.Empty, 
                    string.Empty,
                    MapNetworkDriveHelper.CONNECT_INTERACTIVE | MapNetworkDriveHelper.CONNECT_PROMPT, 
                    null,
                    null, 
                    null);
            else
                ret = MapNetworkDriveHelper.WNetUseConnection(IntPtr.Zero, 
                    nr, 
                    password, 
                    username, 
                    0, 
                    null, 
                    null, 
                    null);

            if (ret == MapNetworkDriveHelper.NO_ERROR) return null;

            return MapNetworkDriveHelper.ParseError(ret);
        }

        public static string DisconnectRemote(this string remotePath)
        {
            string errorMessage = null;
            int ret = MapNetworkDriveHelper.WNetCancelConnection2(remotePath, MapNetworkDriveHelper.CONNECT_UPDATE_PROFILE, false);
            errorMessage =  MapNetworkDriveHelper.ParseError(ret);

            return errorMessage;
        }
    }
}
