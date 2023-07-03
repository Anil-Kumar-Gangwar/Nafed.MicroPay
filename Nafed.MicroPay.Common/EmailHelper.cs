//using EASendMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace Nafed.MicroPay.Common
{
    public class EmailHelper : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Send Email Method 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// 

        //public static bool SendEmail(EmailMessage message)
        //{
        //    log.Info($"From Address {message.From}");
        //    log.Info($"To address {message.To}");
        //    log.Info($"CC address {message.CC}");
        //    log.Info($"BCC address {message.Bcc}");
        //    log.Info($"Friendly Name {message.FriendlyName}");
        //    log.Info($"Smtp Client Host {message.SmtpClientHost}");

        //    try
        //    {
        //        SmtpMail oMail = new SmtpMail("TryIt");
        //        // Set sender email address, please change it to yours
        //        oMail.From = message.From;
        //        // Set recipient email address, please change it to yours
        //        oMail.To = message.To;
        //        // Set email subject
        //        oMail.Subject = message.Subject;
        //        // Set email body
        //        //    oMail.TextBody = message.Body;

        //        oMail.HtmlBody = message.Body; ;
        //        // Your SMTP server address
        //        SmtpServer oServer = new SmtpServer(message.SmtpClientHost);
        //        // User and password for ESMTP authentication, if your server doesn't require
        //        // User authentication, please remove the following codes.
        //        oServer.User = message.UserName;
        //        oServer.Password = message.Password;
        //        // Set 465 SMTP port
        //        oServer.Port = message.SmtpPort;
        //        // Enable SSL connection
        //        oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
        //        EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
        //        oSmtp.SendMail(oServer, oMail);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Email Error-" + ex.Message + ", StackTrace-" + ex.StackTrace + ", DateTimeStamp-" + DateTime.Now);
        //        throw;
        //    }
        //}

        //public static bool SendEmail(EmailMessage message)
        //{
        //    log.Info($"From Address {message.From}");
        //    log.Info($"To address {message.To}");
        //    log.Info($"CC address {message.CC}");
        //    log.Info($"BCC address {message.Bcc}");
        //    log.Info($"Friendly Name {message.FriendlyName}");
        //    log.Info($"Smtp Client Host {message.SmtpClientHost}");
        //    using (MailMessage email = new MailMessage())
        //    {
        //        //email.From = new MailAddress(message.From, message.FriendlyName);

        //        email.From = new MailAddress("microwaretest@gmail.com", message.FriendlyName);

        //        foreach (var toAddress in message.To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            email.To.Add(toAddress);
        //        }

        //        if (!String.IsNullOrEmpty(message.CC))
        //        {
        //            foreach (var ccAddress in message.CC.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
        //            {
        //                email.CC.Add(ccAddress);
        //            }
        //        }

        //        if (!String.IsNullOrEmpty(message.Bcc))
        //        {
        //            foreach (var bccAddress in message.Bcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
        //            {
        //                email.Bcc.Add(bccAddress);
        //            }
        //        }
        //        foreach (var attach in message.Attachments)
        //        {
        //            Attachment attachment;
        //            if (attach.Content != null)
        //            {
        //                attachment = AddAttachment(attach.Content, attach.FileName);
        //            }
        //            else
        //            {
        //                attachment = AddAttachment(attach.FileName);
        //            }
        //            email.Attachments.Add(attachment);
        //        }
        //        email.IsBodyHtml = message.HTMLView;
        //        email.Subject = message.Subject;
        //        email.Body = message.Body;
        //        email.Priority = message.MailPriority;

        //        #region Set Embedded Content
        //        if (message.EmbeddedContent != null)
        //        {
        //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, Encoding.UTF8, MediaTypeNames.Text.Html);
        //            AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(message.Body, "<[^>]+?>", string.Empty), Encoding.UTF8, MediaTypeNames.Text.Plain);
        //            string mediaType = MediaTypeNames.Image.Jpeg;
        //            foreach (var ec in message.EmbeddedContent)
        //            {
        //                LinkedResource img = new LinkedResource(ec.Path, mediaType);
        //                img.ContentId = ec.ContentID;
        //                img.ContentType.MediaType = mediaType;
        //                img.TransferEncoding = TransferEncoding.Base64;
        //                img.ContentType.Name = img.ContentId;
        //                img.ContentLink = new Uri("cid:" + img.ContentId);
        //                htmlView.LinkedResources.Add(img);
        //            }
        //            email.AlternateViews.Add(plainView);
        //            email.AlternateViews.Add(htmlView);
        //        }
        //        #endregion
        //        SmtpClient client = new SmtpClient();
        //        //client.Host = message.SmtpClientHost;
        //        //client.Host = "smtp.gmail.com";
        //        //NetworkCredential basicCredential = new NetworkCredential("microwaretest@gmail.com", "Microtest@123");
        //        //client.Credentials = basicCredential;
        //        // client.Port = 587;
        //        //client.EnableSsl = true; // message.enableSSL;
        //        //client.UseDefaultCredentials = true;

        //        client.Host = "smtp.gmail.com";
        //        NetworkCredential basicCredential1 = new NetworkCredential("microwaretest@gmail.com", "Microtest@123");
        //        client.Credentials = basicCredential1;
        //        client.Port = 587;
        //        client.EnableSsl = true;
        //        client.UseDefaultCredentials = false;

        //        try
        //        {
        //            client.Send(email);
        //            log.Info("Email Sent-" + message.To + ", DateTimeStamp - " + DateTime.Now);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("Email Error-" + ex.Message + ", StackTrace-" + ex.StackTrace + ", DateTimeStamp-" + DateTime.Now);
        //            throw;
        //        }
        //    }
        //}
        /// <summary>
        /// EMAILTEST METHOD --
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendEmail(EmailMessage message)
        {
            log.Info($"From Address {message.From}");
            log.Info($"To address {message.To}");
            log.Info($"CC address {message.CC}");
            log.Info($"BCC address {message.Bcc}");
            log.Info($"Friendly Name {message.FriendlyName}");
            log.Info($"Smtp Client Host {message.SmtpClientHost}");
            using (MailMessage email = new MailMessage())
            {
                email.From = new MailAddress(message.From, message.FriendlyName);
                foreach (var toAddress in message.To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    email.To.Add(toAddress);
                }

                if (!String.IsNullOrEmpty(message.CC))
                {
                    foreach (var ccAddress in message.CC.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        email.CC.Add(ccAddress);
                    }
                }

                if (!String.IsNullOrEmpty(message.Bcc))
                {
                    foreach (var bccAddress in message.Bcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        email.Bcc.Add(bccAddress);
                    }
                }
                foreach (var attach in message.Attachments)
                {
                    Attachment attachment;
                    if (attach.Content != null)
                    {
                        attachment = AddAttachment(attach.Content, attach.FileName);
                    }
                    else
                    {
                        attachment = AddAttachment(attach.FileName);
                    }
                    email.Attachments.Add(attachment);
                }
                email.IsBodyHtml = true;
                email.Subject = message.Subject;
                email.Body = message.Body;
                email.Priority = message.MailPriority;
                //    email.From = new MailAddress(message.From,message.FriendlyName);

                #region Set Embedded Content
                if (message.EmbeddedContent != null)
                {
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, Encoding.UTF8, MediaTypeNames.Text.Html);
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(message.Body, "<[^>]+?>", string.Empty), Encoding.UTF8, MediaTypeNames.Text.Plain);
                    string mediaType = MediaTypeNames.Image.Jpeg;
                    foreach (var ec in message.EmbeddedContent)
                    {
                        LinkedResource img = new LinkedResource(ec.Path, mediaType);
                        img.ContentId = ec.ContentID;
                        img.ContentType.MediaType = mediaType;
                        img.TransferEncoding = TransferEncoding.Base64;
                        img.ContentType.Name = img.ContentId;
                        img.ContentLink = new Uri("cid:" + img.ContentId);
                        htmlView.LinkedResources.Add(img);
                    }
                    email.AlternateViews.Add(plainView);
                    email.AlternateViews.Add(htmlView);
                }
                #endregion
                SmtpClient client = new SmtpClient(message.SmtpClientHost);
                NetworkCredential basicCredential = new NetworkCredential(message.UserName, message.Password);

                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential;
                client.Port = message.SmtpPort;
                client.EnableSsl = message.enableSSL;
                client.Host = message.SmtpClientHost;               
                client.ServicePoint.MaxIdleTime = 1;               
                var client1 = new WebClient();
                bool internetCon;

                try
                {
                    client.Send(email);
                    log.Info("Email Sent-" + message.To + ", DateTimeStamp - " + DateTime.Now);
                    return true;
                }
                catch (Exception ex)
                {

                    using (client1.OpenRead("http://google.com"))
                        internetCon = true;

                    log.Error("Email Error-" + ex.InnerException + ", StackTrace-" + ex.StackTrace + ", DateTimeStamp-" + DateTime.Now);
                    log.Error("Internet Connection-" + internetCon + ", StackTrace-" + ex.StackTrace + ", DateTimeStamp-" + DateTime.Now);

                    throw;
                }
            }


        }
        private static Attachment AddAttachment(string file)
        {
            Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            return attachment;
        }
        private static Attachment AddAttachment(Stream ms, string sFileName)
        {
            Attachment attachment = new Attachment(ms, sFileName);
            return attachment;
        }

        public void Dispose()
        {
        }

    }
    /// <summary>
    /// Email Message Class
    /// </summary>
    public class EmailMessage
    {
        public string UserName { set; get; }
        public string Password { set; get; }
        public string SmtpClientHost { set; get; }
        public int SmtpPort { set; get; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FriendlyName { get; set; }
        public string CC { get; set; }
        public string Bcc { set; get; }
        public bool enableSSL { set; get; }
        public bool HTMLView { set; get; }
        public List<MailAttachment> Attachments { set; get; }
        public System.Net.Mail.MailPriority MailPriority { set; get; }
        public EmailMessage()
        {
            Attachments = new List<MailAttachment>();
        }
        public List<EmbeddedContent> EmbeddedContent { set; get; }
    }
    /// <summary>
    /// Mail Attachment Class
    /// </summary>
    public class MailAttachment
    {
        public string FileName { get; set; }
        public Stream Content { get; set; }
    }

    public class EmbeddedContent
    {
        public string ContentID { get; set; }
        public string Path { get; set; }
    }

    public class EmailHeaderFooter
    {
        public static string Header = "<div style='background-color:{0}; height:50px; width:100%; color:white; display: inline-block;'><img src='cid:{1}' height='50'/></div>";
        public static string Footer = "<div style='background-color:{0}; height:50px; width:100%; color:white;'><font face='Tahoma' size='2'><p>Best Regards,</p><font face='Tahoma' size='2'><p>WorkstreamX Support Team</p></div>";
    }




}
