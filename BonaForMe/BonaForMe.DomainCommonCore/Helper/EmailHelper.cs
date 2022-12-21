using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BonaForMe.DomainCommonCore.Helper
{
    public class EmailHelper
    {
        public static void SendForgetPasswordMail(string Email, string newPassword)
        {
            string bodyMessage = GetMailBody("Bona Me For Me Password Reset", "Bona Me For Me Your new temporary user password is given below.</br> For your security, please change your password! </br>", newPassword);

            var message = new System.Net.Mail.MailMessage("info@bonameforme.com", Email)
            {
                Subject = "Bona Me For Me User Password Renewal Process",
                Body = bodyMessage

            };

            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            message.Priority = MailPriority.High;

            var client = new System.Net.Mail.SmtpClient();
            client.EnableSsl = false;
            client.Host = "mail.bonameforme.com";
            client.Port = 587;
            client.Credentials = new NetworkCredential("info@bonameforme.com", "Bgw9v457^");
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            client.Send(message);
        }

        public static string GetMailBody(string baslik, string icerik, string sifre) 
        {
            var content = "";
            var path = Path.Combine(Directory.GetCurrentDirectory()) + @"\";
            var fullPath = path + "Views/Shared/EmailTemplate.html";
            using (StreamReader streamReader = new StreamReader(fullPath, Encoding.UTF8))
            {
                content = streamReader.ReadToEnd();
            }
            content = content.Replace("|Baslik|", baslik);
            content = content.Replace("|Icerik|", icerik);
            content = content.Replace("|Sifre|", sifre);
            return content; 
        }
    }
}
