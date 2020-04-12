using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Text;
using System.Diagnostics;

using McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient;

namespace McAfeeLabs.Engineering.Automation.Base
{
    static public class SMSSampleDownloadExtention
    {
        public static bool SMSDownload( this string hash,
                                        string localFolder,
                                        string extesion = null,
                                        StringBuilder errorMessages = null,
                                        int retryNumber = 3,
                                        bool throwException = false)
        {
            var success = false;
            var filepathFormat = string.IsNullOrEmpty(extesion) ? @"{0}/{1}" : @"{0}/{1}.{2}";
            var request = string.Format("<request requestor=\"\"><criteria><criterion key=\"md5\"><![CDATA[{0}]]></criterion></criteria><options><option key=\"RetreiveBinary\"><![CDATA[True]]></option><option key=\"CompressSample\"><![CDATA[false]]></option></options></request>", 
                                        hash);

            for (int retry = 1; retry < retryNumber && !success; retry++)
            {
                var smsServiceClient = new SampleManagementServiceSoapClient();

                if (hash.StartsWith("0x"))
                    hash = hash.Remove(0, 2);
                try
                {
                    var response = smsServiceClient.RetrieveSamples(request);

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(response);
                    XmlNode contentNode = xmlDoc.SelectSingleNode("/response/samples/sample/binary");
                    if (null == contentNode)
                    {
                        if (null != errorMessages)
                            errorMessages.AppendLine(@"---Download sample failed, could not find the binary content");
                        break;
                    }

                    XmlNode statusNode = xmlDoc.SelectSingleNode("response/status");
                    XmlAttribute statusAttr = statusNode.Attributes["error"];

                    if (statusAttr.Value.ToUpper() != "FALSE")
                    {
                        if (null != errorMessages)
                            errorMessages.AppendLine(@"---Download sample failed, the error response status code caught.");
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
                    smsServiceClient.Abort();

                    Trace.WriteLine(string.Format(@"---Exception caught at downloading hash {0} from SMS, endpoint<{1}>, error was : {2}", hash, smsServiceClient.Endpoint.Address.Uri, ex.Message));
                    if (throwException)
                        throw ex;
                }
            }
            return success;
        }
    }
}
