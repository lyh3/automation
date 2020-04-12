using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.IO;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    abstract public class FTPClient : IComparable
    {
        #region Delclarations

        protected const char PathSeparator = '/';
        private bool _passivemode = false;
        private const string Urlpatterns = @"(^|[ \t\r\n])((ftp|http|https|gopher|mailto|news|nntp|telnet|wais|file|prospero|aim|webcal):(([A-Za-z0-9$_.+!*(),;/?:@&~=-])|%[A-Fa-f0-9]{2}){2,}(#([a-zA-Z0-9][a-zA-Z0-9$_.+!*(),;/?:@&~=%-]*))?([A-Za-z0-9$_+!*();/?:~-]))";
        protected string _lastErrorMessage;
        protected const string ErrorMessageFormat = @"Exception caught at <{0}> existing, error was:{1}, trace : {2}";
        protected bool _isBubbleUpException = false;
        protected Exception _lastexception;
        protected string _ftpuri;
        protected string _host;
        protected string _username;
        protected string _password;

        #endregion

        #region Constructors

        public FTPClient() { }

        public FTPClient(string uri)
        {
            Initialize(uri);
        }

        #endregion

        #region Properties

        public string FtpUri { get { return _ftpuri; } }
        public string Host { get { return _host; } }
        public string UserName { get { return _username; } }
        public string PassWord { get { return _password; } }
        public int? TimeOut { get; set; }

        public bool PassiveMode
        {
            get { return _passivemode; }
            set { _passivemode = value; }
        }
        public bool IsBubbleUpException
        {
            get { return _isBubbleUpException; }
            set { _isBubbleUpException = value; }
        }
        public string LastErrorMessage { get { return _lastErrorMessage; } }


        #endregion

        #region Public Methods

        abstract public void Initialize(string uri);
        abstract public int CompareTo(object obj);

        public string GetParentDirectory()
        {
            var parentDir = string.Empty;

            if (!string.IsNullOrEmpty(FtpUri))
            {
                var idx = FtpUri.LastIndexOf(PathSeparator);
                parentDir = FtpUri.Substring(0, idx);
            }

            return parentDir;
        }

        public bool Ping()
        {
            var results = false;
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.PrintWorkingDirectory);
            string ErrorMessageHeader = string.Format(@"Ping <{0}>", this.FtpUri);
            try
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    if (null != responseStream)
                    {
                        results = true;
                    }
                    response.Close();
                }
            }
            catch (WebException webex)
            {
                _lastexception = webex;
                _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                   ErrorMessageHeader,
                                                   null != webex.InnerException ? webex.InnerException.Message : webex.Message,
                                                   webex.StackTrace);
            }
            catch (Exception ex)
            {
                _lastexception = ex;
                _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                   ErrorMessageHeader,
                                                   null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                   ex.StackTrace);
            }

            return results;
        }

        public static string ComposeInitialUriString(string uri,
                                                         string userName,
                                                         string password)
        {

            var uriDetails = new UriBuilder(uri);
            return string.Format(@"{0}{1}:{2}@{3}",
                              GlobalDefinitions.FtpPrefix,
                              userName,
                              password,
                              uri.Replace(GlobalDefinitions.FtpPrefix, string.Empty));
        }

        #endregion

        #region Private Methods

        protected void InitializeUri(string uri)
        {
            ValidUriString(uri);
            ParseFormatedUri(uri);
        }

        private void ParseFormatedUri(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var split = Regex.Split(uri, GlobalDefinitions.FtpPrefix);
                if (split.Length > 1)
                {
                    var idx = split[1].IndexOf('@');
                    _ftpuri = string.Format(@"{0}{1}", GlobalDefinitions.FtpPrefix, split[1].Substring(idx + 1, split[1].Length - idx - 1));
                    if (-1 != idx)
                    {
                        var login = split[1].Substring(0, idx).Split(new[] { ':' });
                        if (login.Length > 0)
                        {
                            _username = login[0];
                            _password = login[1];
                        }
                    }
                }
            }
        }

        protected void ValidUriString(string uri)
        {
            bool isValid = string.IsNullOrEmpty(uri) == false;
            if (isValid)
                isValid = new Regex(Urlpatterns, RegexOptions.Compiled).Match(uri).Success;

            if (!isValid)
                throw new ArgumentException(@"The ftp uri is invalid");
        }

        protected FtpWebRequest CreateRequest(string method,
                                              string target = null,
                                              Boolean keepAlive = false)
        {
            FtpWebRequest request = null;

            var requestFormat = string.IsNullOrEmpty(target) ? @"{0}" : @"{0}/{1}";
            request = (FtpWebRequest)FtpWebRequest.Create(string.Format(requestFormat, FtpUri, target));
            request.Credentials = new NetworkCredential(UserName, PassWord);
            request.KeepAlive = keepAlive;
            if (null != TimeOut) request.Timeout = TimeOut.Value;
            request.UseBinary = true;
            request.Method = method;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.UsePassive = _passivemode;
            request.ServicePoint.ConnectionLimit = 1;

            return request;
        }

        #endregion
    }
}
