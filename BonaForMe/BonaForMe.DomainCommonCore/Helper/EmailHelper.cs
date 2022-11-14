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
            string bodyMessage = GetMailBody("Bona For Me Şifre Yenileme", "mmcard.online yeni geçici kullanıcı şifreniz aşağıda verilmiştir.</br> Güvenliğiniz için lütfen şifrenizi değiştiriniz! </br>", newPassword);

            var message = new System.Net.Mail.MailMessage("info@mmcard.online", Email)
            {
                Subject = "mmcard Kullanıcı Şifre Yenileme İşlemi.",
                Body = bodyMessage

            };

            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            message.Priority = MailPriority.High;

            var client = new System.Net.Mail.SmtpClient();
            client.EnableSsl = false;
            client.Host = "mail.mmcard.online";
            client.Port = 587;
            client.Credentials = new NetworkCredential("info@mmcard.online", "y3wfpt-kC1vu@Hd.");
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            client.Send(message);
        }

        public static string GetMailBody(string baslik, string icerik, string sifre) 
        {
            var content = "";
            using (StreamReader streamReader = new StreamReader("Views/Shared/EmailTemplate.html", Encoding.UTF8))
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
