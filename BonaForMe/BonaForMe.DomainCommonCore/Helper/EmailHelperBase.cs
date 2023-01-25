using static BonaForMe.DomainCommonCore.Enums.AppEnums;

namespace BonaForMe.DomainCommonCore.Helper
{
    public class EmailHelperBase
    {
        public static string GetEmailTemplates(MailTypes mailTypes, string notification = "")
        {
            switch (mailTypes)
            {
                case MailTypes.Welcome:
                    return GetMailBody("", "", notification);
                case MailTypes.ForgetPassword:
                    return GetMailBody(
                        "Bona Me For Me Password Reset",
                        "Bona Me For Me Your new temporary user password is given below.</br> For your security, please change your password! </br>",
                        notification);
                case MailTypes.MobileApprove:
                    return GetMailBody("", "", notification);
                case MailTypes.InvoiceDelivery:
                    return GetMailBody("", "", notification);
                default:
                    return "";
            }
        }

        public static string GetMailBody(string baslik, string icerik, string notification)
        {
            string content = MailTemplate;
            content = content.Replace("|Baslik|", baslik);
            content = content.Replace("|Icerik|", icerik);
            content = content.Replace("|Notification|", GetNotification(notification));
            return content;
        }

        public static string GetNotification(string icerik)
        {
            return string.IsNullOrEmpty(icerik) ? string.Empty : Notification.Replace("|Icerik|", icerik);
        }

        private static string Notification = @"
            <a href='javascript:void(0);' 
                style='background:#20e277;
                    text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff; 
                    font-size:14px; padding:10px 24px; display:inline-block; border-radius:50px;'>
                    |Icerik|</a>
                ";

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
                                                    |Notification|
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