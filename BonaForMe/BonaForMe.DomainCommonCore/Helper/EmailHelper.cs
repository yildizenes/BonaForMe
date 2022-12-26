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
            string bodyMessage = GetMailBody("Bona Me For Me Password Reset", "Bona Me For Me Your new temporary user password is given below.</br> For your security, please change your password! </br>", newPassword, MailTemplate);

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

        public static string GetMailBody(string baslik, string icerik, string sifre, string content)
        {
            content = content.Replace("|Baslik|", baslik);
            content = content.Replace("|Icerik|", icerik);
            content = content.Replace("|Sifre|", sifre);
            return content;
        }

        private static string MailTemplate = @"
                <table cellspacing='0' border='0' cellpadding='0' width='100%' bgcolor='#f2f3f8' style='@import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;'>
                    <tr>
                        <td>
                            <table style = 'background-color: #f2f3f8; max-width:670px;  margin:100px auto;' width='100%' border='0' align='center' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td>
                                        <table width = '95%' border='0' align='center' cellpadding='0' cellspacing='0'
                                               style='max-width:670px; background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);'>
                                            <tr style = 'margin: 100px;' >
                                                <td style='padding:35px;'>
                                                    <h1 style = 'color:#1e1e2d; font-weight:500; margin:0;font-size:32px;font-family:'Rubik',sans-serif;' >|Baslik|</ h1 >
                                                    <span style='display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; width:100px;'></span>
                                                    <p style = 'color:#455056; font-size:15px;line-height:24px; margin:0;' >
                                                        |Icerik|
                                                    </p>
                                                    <a href='javascript:void(0);' style='background:#20e277;text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff; font-size:14px;padding:10px 24px;display:inline-block;border-radius:50px;'>|Sifre|</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                            </table>
                        </td>
                    </tr>
                </table>
            ";
    }
}
