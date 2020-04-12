using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

using log4net;

namespace Automation.Base.BuildingBlocks
{
    public class EmailService
    {
        public const string DateDispFormatMMss = @"yyyy-MM-dd HH:mm:ss";
        public bool SendEmail(string smtpserver, 
                              string fromAddress, 
                              string toAddress, 
                              string subject, 
                              string body)
        {
            using (var message = new MailMessage(new MailAddress(fromAddress), new MailAddress(toAddress))
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    SendMail(smtpserver, message);
                    return true;
                }
                catch (ConfigurationException ex)
                {
                    if (null != Logger)
                        Logger.Error(string.Format(@"Exception caught from send email, error: ", ex.Message));
                }
            }
            return false;
        }

        public ILog Logger { get; set; }
        public bool SendEmailWithAttachment(string smtpserver,
                                            string fromAddress,
                                            string toAddress, 
                                            string subject, 
                                            string body, 
                                            string attachmentContent, 
                                            string fileName)
        {
            using (var message = new MailMessage(new MailAddress(fromAddress), new MailAddress(toAddress))
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
                            SendMail(smtpserver, message);
                            return true;
                        }
                        catch (ConfigurationException ex)
                        {
                            if (null != Logger)
                                Logger.Error(string.Format(@"Exception caught from send email, error: {0}", ex.Message));
                        }
                    }
                }
                finally
                {
                    if (null != attachement)
                        attachement.Dispose();
                }
            }
            return false;
        }

        private void SendMail(string strStmpCliet, MailMessage message)
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
    }
}
