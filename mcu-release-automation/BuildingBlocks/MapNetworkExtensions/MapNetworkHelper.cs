using System.Collections.Generic;
using System.Xml;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class MapNetworkHelper
    {
        #region Declarations

        private List<string> _remoteConnectionList = new List<string>();

        #endregion

        #region Public Methods

        public void ConnectRemoteSource(XmlDocument configXmlDoc, string [] subFolders)
        {
            const string RemoteConnectionFormat = @"\\{0}\{1}";
            var xpath = @"/*[local-name()='configuration']/*[local-name()='RemoteConnections']/*[local-name()='RemoteConnection']";
            var connectionNodes = configXmlDoc.SelectNodes(xpath);
            var xmlDoc = new XmlDocument();

            foreach (XmlNode node in connectionNodes)
            {
                xmlDoc.LoadXml(node.OuterXml);
                var account = xmlDoc.SelectSingleNode(@"/RemoteConnection/@account").InnerText;
                var ipaddress = xmlDoc.SelectSingleNode(@"/RemoteConnection/@ipaddress").InnerText;
                var username = xmlDoc.SelectSingleNode(@"/RemoteConnection/@username").InnerText;
                var password = xmlDoc.SelectSingleNode(@"/RemoteConnection/@password").InnerText;
                string remoteconnection = string.Format(RemoteConnectionFormat, ipaddress, account);
                if(string.IsNullOrEmpty(remoteconnection.ConnectToRemote(username, password)))
                    _remoteConnectionList.Add(remoteconnection);
                foreach (var subfolder in subFolders)
                {
                    var remotepath = string.Format(@"{0}\{1}", remoteconnection, subfolder);
                    if(string.IsNullOrEmpty(remotepath.ConnectToRemote(username, password)))
                        _remoteConnectionList.Add(remotepath);
                }
            }
        }

        public void DisConnectRemoteSource()
        {
            _remoteConnectionList.ForEach(remotepath => { remotepath.DisconnectRemote(); });
        }

        #endregion
    }
}
