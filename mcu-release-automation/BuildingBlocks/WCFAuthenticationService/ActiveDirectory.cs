using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Collections;
using System.Management;

namespace WCFAuthenticationService
{
    public class ActiveDirectory
    {
        private Dictionary<string, List<string>> _groupDictionary = new Dictionary<string, List<string>>();
        static private DirectorySearcher _searcher = null;
        public string Domain { get; set; }
        public string Hoset { get; set; }
        public bool Available { get; set; }

        public ActiveDirectory()
        {
            this.Hoset = System.Net.Dns.GetHostName();
            _Initialize();
        }

        public ActiveDirectory(string domain) : this()
        {
            Domain = domain;
        }

        #region Public Method

        public string[] GroupCollection
        {
            get
            {
                string[] groupCollection = null;

                if (null != _groupDictionary)
                {
                    groupCollection = _groupDictionary.Keys.ToArray<string>();
                }

                return groupCollection;
            }
        }

        public string[] GroupMembers(string group)
        {
            return _groupDictionary.ContainsKey(group) ? _groupDictionary[group].ToArray() : null;
        }

        public bool IsMemmberOf(string group, string longinName)
        {
            bool isMemeber = false;

            string[] members = this.GroupMembers(group);

            if (null != members)
            {
                List<string> memberList = new List<string>();
                memberList.AddRange(members);
                string userName = SearchUserName(longinName);

                if (null != userName)
                {
                    isMemeber = memberList.Contains(userName);
                }
            }

            return isMemeber;
        }

        public string SearchUserName(string loginName)
        {
            string userName = null;

            if (null != _searcher)
            {
                string filter = string.Format("(sAMAccountName={0})", loginName); ;
                _searcher.Filter = filter;
                SearchResult adSearchResult = _searcher.FindOne();
                if (null != adSearchResult)
                {
                    DirectoryEntry entry = adSearchResult.GetDirectoryEntry();
                    if (null != entry)
                    {
                        userName = entry.Name.Replace("CN=", string.Empty);
                    }
                }
            }

            return userName;
        }
        #endregion

        #region Private Method

        private string SearchDomainName()
        {
            string domainName = string.Empty;

            SelectQuery query = new SelectQuery("Win32_ComputerSystem");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject managementObj in searcher.Get())
                {
                    if ((bool)managementObj["partofdomain"] != true)
                    {
                        string workgroup = (managementObj["workgroup"]).ToString();
                    }
                    else
                    {
                        this.Domain = (managementObj["domain"]).ToString();
                    }
                }
            }

            return domainName;
        }

        private void _Initialize()
        {
            if(string.IsNullOrEmpty(this.Domain))
            {
                this.Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (string.IsNullOrEmpty(this.Domain))
                {
                    this.Domain = this.SearchDomainName();
                }
            }

            if (string.IsNullOrEmpty(this.Domain))
            {
                this.Available = false;
                return;
            }

            for (int i = 0; i < 3 && !this.Available; ++i)
            {
                DirectoryEntry ldapEntry = new DirectoryEntry(string.Format("LDAP://{0}", Domain));
                _searcher = new DirectorySearcher(ldapEntry);

                string filter = "(&(objectClass=group))";
                string s = string.Empty;
                _searcher.Filter = filter; 
                
                try
                {
                    foreach (SearchResult result in _searcher.FindAll())
                    {
                        s = string.Format("--- group name = <{0}>, path = <{1}>, guid = <{2}>{3}",
                            result.GetDirectoryEntry().Name,
                            result.GetDirectoryEntry().Path,
                            result.GetDirectoryEntry().NativeGuid,
                            Environment.NewLine);
                        System.Diagnostics.Trace.WriteLine(s);

                        DirectoryEntry dir = result.GetDirectoryEntry();
                        object entries = dir.Invoke("members", null);
                        string groupName = result.GetDirectoryEntry().Name.Replace("CN=", string.Empty);
                        List<string> memberList = new List<string>();
                        foreach (object groupMember in (IEnumerable)entries)
                        {
                            DirectoryEntry member = new DirectoryEntry(groupMember);
                            s = string.Format("--- User name = <{0}>{1}", member.Name, Environment.NewLine);
                            System.Diagnostics.Trace.WriteLine(s);
                            memberList.Add(member.Name.Replace("CN=", string.Empty));
                        }

                        if (!_groupDictionary.ContainsKey(groupName))
                        {
                            _groupDictionary.Add(groupName, memberList);
                        }
                    }
                    this.Available = true;
                }
                catch { System.Threading.Thread.Sleep(2000); }
            }
        }

        #endregion
    }
}
