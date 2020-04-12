using System;
using System.Linq;
using NUnit.Framework;
using AccessPrivilege;
using WCFAuthenticationService;
using client = WcfAuthenticationClient.WcfAuthenticationService;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [TestFixture]
    [AccessPrivilegeAdmin, AccessPrivilegeDeveloper]
    public class WcfAuthenticationServiceTest
    {
        [Test]
        public void AccessPermittedTest()
        {
            var privilegeList = this.GetType().GetCustomAttributes(typeof(AccessPrivilegeAttribute), true) as AccessPrivilegeAttribute[];
            var privileges = 0x0;
            foreach (var p in privilegeList)
                privileges += (int)p.AccessPrivilegeGroup;
            var requestData = new client.AuthorizationRequestData()
            {
                LoginName = @"liyingho",//Environment.UserName,
                PrivilegeGroup = (client.AccessPrivilegeGroup)privileges,
            };

            var authenticationClient = new client.AuthenticationClient();
            var results = authenticationClient.AuthorizationRequest(requestData);

            //var results = AccessAuthentication.Instance.Authentication.AuthorizationRequest(requestData);
            Assert.IsTrue(null != results);
        }

        [Test]
        public void ActiveDirectorySearchDomainTest()
        {
            var activeDirectory = new ActiveDirectory();
        }

        [Test]
        public void ActiveDirectorySearchUserTest()
        {
            const string domain =  @"amr.corp.intel";
            const string loginName = @"liyingho";
            var activeDirectory = new ActiveDirectory(domain);
            Assert.AreEqual(loginName, activeDirectory.SearchUserName(loginName));
        }

        [Test]
        public void ActiveDirectoryIsMemberOfTest()
        {
            const string loginName = @"liyingho";
            var activeDirectory = new ActiveDirectory();
            var mcuadmin = activeDirectory.GroupCollection.FirstOrDefault(x => x == "mcu_administrators");
            Assert.IsTrue(activeDirectory.IsMemmberOf(@"mcu_administrators ", loginName));
            Assert.IsTrue(activeDirectory.IsMemmberOf(@"world.gs.avert.engineering.admins", loginName));
        }
    }
}
