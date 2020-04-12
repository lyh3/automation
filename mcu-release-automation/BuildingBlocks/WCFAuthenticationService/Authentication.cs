using System.ServiceModel;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Web.Hosting;
using System.Xml;

using AccessPrivilege;

namespace WCFAuthenticationService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Authentication : IAuthentication
    {
        const string AccessprivilegeGroupConfigXpath = @"/*[local-name()='configuration']/*[local-name()='AccessPrivilegeGroups']/*[local-name()='AccessPrivilegeGroup']";
        private ActiveDirectory activeDirectory = null;
        private ConcurrentDictionary<string, string> _configuredProupDictionary = new ConcurrentDictionary<string, string>();

        public Authentication()
        {
            var configFileName = HostingEnvironment.MapPath("~/Web.config");
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(configFileName);
            foreach (XmlNode xmlNode in xmlDoc.SelectNodes(AccessprivilegeGroupConfigXpath))
            {
                var key = xmlNode.Attributes["key"].Value;
                var accessPrivilegeGroup = xmlNode.Attributes["value"].Value;
                if(!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(accessPrivilegeGroup))
                    _configuredProupDictionary.AddOrUpdate(key, accessPrivilegeGroup, (x, value) => value = accessPrivilegeGroup);
            }
        }

        public string Ping()
        {
            return "Echo from Authentication service";
        }

        private ActiveDirectory AD
        {
            get
            {
                if (this.activeDirectory == null)
                {
                    activeDirectory = new ActiveDirectory();
                }

                return this.activeDirectory;
            }
        }

        public AuthorizationResults AuthorizationRequest(AuthorizationRequestData requestData)
        {
            var priviegeGroups = new[] { AccessPrivilegeGroup.ALL, AccessPrivilegeGroup.GUEST, AccessPrivilegeGroup.USER, AccessPrivilegeGroup.DEVELOPER, AccessPrivilegeGroup.SYSTEM_ADMIN };
            var accessPrivilegeGroupList = new List<string>();
            foreach (var g in priviegeGroups)
            {
                if (0 != (requestData.PrivilegeGroup & g))
                    accessPrivilegeGroupList.Add(g.ToString());
            }

            AuthorizationResults results = new AuthorizationResults()
            {
                Acknowledged = false
            };

            if (this.AD.Available)
            {              
                foreach (var accessPrivilegeGroup in accessPrivilegeGroupList)
                {
                    string group = string.Empty;
                    _configuredProupDictionary.TryGetValue(accessPrivilegeGroup, out group);
                    if (this.AD.IsMemmberOf(group, requestData.LoginName))
                    {
                        results.Acknowledged = true;
                        break;
                    }
                }
                if (results.Acknowledged == false)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < accessPrivilegeGroupList.Count; ++i)
                    {
                        sb.Append(accessPrivilegeGroupList[i]);
                        if (accessPrivilegeGroupList.Count > 1 && i < accessPrivilegeGroupList.Count - 1)
                        {
                            sb.Append(" or ");
                        }
                    }
                    results.Errormessage = string.Format(@"The login account <{0}> needs to have access privilege of group <{1}>", requestData.LoginName, sb.ToString());
                }
            }
            else
            {
                results.Errormessage = "Failed to access this function due to the Active directory is not available. Please verify the domain configuration.";
            }

            return results;
        }
    }
}
