using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient;

namespace McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy
{
    static public class SmartSMSSampleDownloadExtention
    {
        [ThreadStatic]
        private static SMSProxyController _smsProxyController;
        [ThreadStatic]
        private static SampleManagementServiceSoap _smsProxy;

        public static bool SmartSMSDownload(this string hash,
                                            string localFolder,
                                            string extesion = null,
                                            StringBuilder errorMessages = null,
                                            int retryNumber = 3,
                                            bool throwException = false)
        {
            var success = false;
            var filepathFormat = string.IsNullOrEmpty(extesion) ? @"{0}/{1}" : @"{0}/{1}.{2}";
            var request = string.Format("<request requestor=\"\"><criteria><criterion key=\"md5\"><![CDATA[{0}]]></criterion></criteria><options><option key=\"RetreiveBinary\"><![CDATA[True]]></option><option key=\"CompressSample\"><![CDATA[True]]></option></options></request>",
                                        hash);
            if (null == _smsProxyController)
            {
                _smsProxyController = new SMSProxyController(@"McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap");
                _smsProxy = _smsProxyController.ServiceProxy;
            }

            for (int retry = 1; retry < retryNumber && !success; retry++)
            {
                if (hash.StartsWith("0x")) 
                    hash = hash.Remove(0, 2);

                try
                {
                    RetrieveSamplesResponse response = _smsProxyController.RetrieveSamples(GetRequest(request), _smsProxy);

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(response.Body.RetrieveSamplesResult);

                    XmlNode statusNode = xmlDoc.SelectSingleNode("response/status");
                    XmlAttribute statusAttr = statusNode.Attributes["error"];

                    if (statusAttr.Value.ToUpper() != "FALSE")
                    {
                        if (null != errorMessages)
                            errorMessages.AppendLine(string.Format(@"---Download sample of md5 <{0}> failed, the error response status code has been caught.", hash));
                        break;
                    }

                    XmlNode contentNode = xmlDoc.SelectSingleNode("/response/samples/sample/binary");
                    if (null == contentNode)
                    {
                        if (null != errorMessages)
                            errorMessages.AppendLine(string.Format(@"---Download sample of md5 <{0}> failed, could not find the binary content", hash));
                        break;
                    }

                    byte[] buffer = Convert.FromBase64String(contentNode.InnerText);
                    var memeStream = new MemoryStream(buffer);
                    using (var outputFileStream = new FileStream(string.Format(filepathFormat, localFolder, hash, extesion), FileMode.Create, FileAccess.Write))
                    {
                        memeStream.CopyTo(outputFileStream);
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format(@"---Exception caught at downloading hash {0} from SMS, endpoint<{1}>, error was : {2}", hash, (_smsProxy as SampleManagementServiceSoapClient).Endpoint.Address.Uri, ex.Message));
                    if (throwException)
                        throw ex;
                }
            }
            return success;
        }

        private static void Dispose(FileStream outputFileStream, FromBase64Transform transform)
        {
            transform.Clear();
            outputFileStream.Close();
        }

        private static RetrieveSamplesRequest GetRequest(string request)
        {
            RetrieveSamplesRequest inValue = new McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequest();
            inValue.Body = new RetrieveSamplesRequestBody();
            inValue.Body.request = request;
            return inValue;
        }
    }
}
