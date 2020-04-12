using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using AccessPrivilege;

namespace WCFAuthenticationService
{
   [ServiceContract]
    public interface IAuthentication
    {
        [OperationContract]
        string Ping();

        [OperationContract]
        AuthorizationResults AuthorizationRequest(AuthorizationRequestData requestData);
    }

    [DataContract]
    public class AuthorizationResults
    {
        private bool? acknowledged = null;
        private string errormessage = string.Empty;

        [DataMember]
        public bool? Acknowledged
        {
            get { return acknowledged; }
            set { acknowledged = value; }
        }

        [DataMember]
        public string Errormessage
        {
            get { return errormessage; }
            set { errormessage = value; }
        }
    }

    [DataContract]
    public class AuthorizationRequestData
    {
        private string domain = string.Empty;
        private string loginName = string.Empty;
        private AccessPrivilegeGroup privilegeGroup = AccessPrivilegeGroup.GUEST;
        private string token = string.Empty;

        [DataMember]
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        [DataMember]
        public string LoginName
        {
            get { return loginName; }
            set { loginName = value; }
        }

        [DataMember]
        public AccessPrivilegeGroup PrivilegeGroup
        {
            get { return privilegeGroup; }
            set { privilegeGroup = value; }
        }

        [DataMember]
        public string Token
        {
            get { return token; }
            set { token = value; }
        }
    }
}
