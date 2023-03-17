using System.Net;
using System.Net.Mail;
using System.Text;

namespace BonaForMe.DomainCommonCore.Helper
{
    public class EmailHelper
    {
        public static Result.Result SendMail(string email, string title, string bodyMessage)
        {
            Result.Result result = new Result.Result();

            var message = new MailMessage("info@bonameforme.com", email)
            {
                Subject = title,
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

            try
            {
                client.Send(message);
                result.Message = "Email sent successfully.";
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }
    }
}