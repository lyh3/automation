using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

using log4net;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class EmailService
    {
        public const string DateDispFormatMMss = @"MM/dd/yyyy HH:mm:ss";
        public const string SMTPserverNameKey = @"SMTPserverName";
        public const string SecureSMTPserverNameKey = @"SecureSMTPserverName";

        public virtual void SendEmail(MailAddress fromAddress, 
                                        MailAddress toAddress, 
                                        string subject, 
                                        string body,
                                        bool secure = false)
        {
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {

                try
                {
                    SendMail(GetSMTPServer(secure), message);
                }
                catch (ConfigurationException ex)
                {
                    if (null != Logger)
                        Logger.Error(string.Format(@"Exception caught from send email, error: ", ex.Message));
                }
            }
        }

        public static ILog Logger { get; set; }

        public virtual void SendEmailWithAttachment(MailAddress fromAddress, 
                                                    MailAddress toAddress, 
                                                    string subject, 
                                                    string body, 
                                                    string attachmentContent, 
                                                    string fileName)
        {
            SendEmailWithAttachment(fromAddress, toAddress, subject, body, attachmentContent, fileName, true);
        }

        public virtual void SendEmailWithAttachment(MailAddress fromAddress,
                                                    MailAddress toAddress, 
                                                    string subject, 
                                                    string body, 
                                                    string attachmentContent, 
                                                    string fileName, 
                                                    bool secure = false)
        {
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                Attachment attachement = null;
                try
                {
                    using (var stream = new MemoryStream(Encoding.Default.GetBytes(attachmentContent)))
                    {
                        attachement = new Attachment(stream, fileName, MediaTypeNames.Text.Plain);
                        var disposition = attachement.ContentDisposition;
                        disposition.FileName = fileName;
                        message.Attachments.Add(attachement);
                        try
                        {
                            SendMail(GetSMTPServer(secure), message);
                        }
                        catch (ConfigurationException ex)
                        {
                            if (null != Logger)
                                Logger.Error(string.Format(@"Exception caught from send email, error: ", ex.Message));
                        }
                    }
                }
                finally
                {
                    if (null != attachement)
                        attachement.Dispose();
                }
            }
        }

        private static void SendMail(string strStmpCliet, MailMessage message)
        {
            try
            {
                if (!string.IsNullOrEmpty(strStmpCliet))
                {
                    using (var client = new SmtpClient(strStmpCliet))
                    {
                        client.Send(message);
                    }
                }
                else
                {
                    if(null != Logger)
                        Logger.Error(string.Format("[{0}]:Failed to send email due to missing Smtp mail server configuration", DateTime.Now.ToString(DateDispFormatMMss)));
                }
            }
            catch (Exception ex)
            {
                if(null != Logger)
                    Logger.Error("Error sending email, error was: " + ex.Message);
            }

        }

        private string GetSMTPServer(bool secure)
        {
            return secure ? ConfigurationManager.AppSettings[SecureSMTPserverNameKey] : ConfigurationManager.AppSettings[SMTPserverNameKey];
        }
    }
}
