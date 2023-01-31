using System.Net;
using System.Net.Mail;
using System.Text;

namespace BonaForMe.DomainCommonCore.Helper
{
    public class EmailHelper
    {
        public static void SendMail(string email, MailTypes mailTypes, string notification)
        {
            string bodyMessage = EmailHelperBase.GetEmailTemplates(mailTypes, notification);
            var message = new MailMessage("info@bonameforme.com", email)
            {
                Subject = "Bona Me For Me User Password Renewal Process",
                Body = bodyMessage,
                IsBodyHtml = true,
                BodyEncoding = Encoding.GetEncoding("utf-8")
                //Priority = MailPriority.High
            };

            var client = new SmtpClient
            {
                EnableSsl = true,
                Host = "mail.bonameforme.com",
                Port = 587,
                Credentials = new NetworkCredential("info@bonameforme.com", "Bgw9v457^"),
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            client.Send(message);
        }
    }
}