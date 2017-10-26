using Lexfy.Web.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Lexfy.Web.Interface
{
    public static class Helpers
    {
        public static string SimpleHashMD5(string password)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                var originalBytes = Encoding.Default.GetBytes(password);
                var hash = md5.ComputeHash(originalBytes);

                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string HashMD5(string password)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(password)).Select(s => s.ToString("x2")));
        }

        public static string HashSHA1(string password)
        {
            try
            {
                var sha1 = SHA1.Create();

                var inputBytes = Encoding.ASCII.GetBytes(password);
                var hash = sha1.ComputeHash(inputBytes);

                var stringBuilder = new StringBuilder();

                foreach (var t in hash)
                    stringBuilder.Append(t.ToString());

                return stringBuilder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static UserViewModel GetAuthenticatedUser()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return new UserViewModel()
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.Name.Split('|')[0]),
                    Name = HttpContext.Current.User.Identity.Name.Split('|')[1],
                    Email = HttpContext.Current.User.Identity.Name.Split('|')[2],
                    SelectedDepartment = HttpContext.Current.User.Identity.Name.Split('|')[3],
                    SelectedUserType = HttpContext.Current.User.Identity.Name.Split('|')[4]
                };
            }

            return null;
        }

        public static bool SendMail(Dictionary<string, string> mails)
        {
            var mailMessage = new System.Net.Mail.MailMessage
            {
                IsBodyHtml = true,
                SubjectEncoding = Encoding.GetEncoding("ISO-8859-1"),
                BodyEncoding = Encoding.GetEncoding("ISO-8859-1"),
                Priority = System.Net.Mail.MailPriority.Normal,
                From = new System.Net.Mail.MailAddress("pressoffice@somainterativa.com.br", "PressOffice",
                    System.Text.Encoding.UTF8)
            };

            foreach (var mail in mails)
            {
                if (mail.Key == "to")
                    mailMessage.To.Add(mail.Value);

                if (mail.Key == "cc")
                    mailMessage.CC.Add(mail.Value);

                if (mail.Key == "replyTo")
                    mailMessage.ReplyToList.Add(mail.Value);

                if (mail.Key == "bcc")
                    mailMessage.Bcc.Add(mail.Value);

                if (mail.Key == "subject")
                    mailMessage.Subject = mail.Value;

                if (mail.Key == "body")
                    mailMessage.Body = mail.Value;
            }

            //Envio
            var objSmtp = new System.Net.Mail.SmtpClient();
            {
                objSmtp.Host = "smtp.gmail.com";
                objSmtp.Port = 587;
                objSmtp.EnableSsl = true;
                objSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //objSmtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                objSmtp.UseDefaultCredentials = false;
                objSmtp.Credentials = new NetworkCredential("pressoffice@somainterativa.com.br", "12@Pre$$OffiC&34");
                //objSmtp.Credentials = CredentialCache.DefaultNetworkCredentials;
                objSmtp.Timeout = 20000;
            }

            try
            {
                objSmtp.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                mailMessage.Dispose();
            }
        }
    }
}