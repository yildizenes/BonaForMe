using BonaForMe.DomainCommonCore.Helper;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.ServiceCore.ApplicationSettingService;
using Microsoft.AspNetCore.Hosting;

namespace BonaForMe.ServiceCore.MailSenderService
{
    public class MailSenderService : IMailSenderService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApplicationSettingService _applicationSettingService;
        public MailSenderService(IWebHostEnvironment webHostEnvironment, IApplicationSettingService applicationSettingService)
        {
            _webHostEnvironment = webHostEnvironment;
            _applicationSettingService = applicationSettingService;
        }

        public Result SendMail(string email, MailTypes mailTypes, string notification)
        {
            var mailText = _applicationSettingService.GetApplicationSetting().Data;
            string title = "";
            string htmlTemplate = @"<html>
                                       <body>
   		                                    |MailBody|
                                      </body>
                                    </html>";

            string bodyMessage = "";

            switch (mailTypes)
            {
                case MailTypes.Welcome:
                    title = "Welcome to Boname Marketing!";
                    //bodyMessage = EmailHelperBase.GetMailBody(mailText.WelcomeMailText, notification);
                    bodyMessage = @"<img src='https://www.boname.ie/images/welcome_boname.jpg' />";
                    break;
                case MailTypes.ForgetPassword:
                    title = "Password Reset";
                    bodyMessage = EmailHelperBase.GetMailBody(mailText.ForgetPasswordMailText, notification);
                    break;
                case MailTypes.MobileApprove:
                    title = "Mobile Approved";
                    bodyMessage = EmailHelperBase.GetMailBody(mailText.MobileApproveMailText, notification);
                    break;
                case MailTypes.InvoiceDelivery:
                    title = "Shopping Invoice";
                    var address = "https://www.boname.ie/Invoice/" + notification + ".pdf";
                    var link = "<a href='" + address + "'>View Invoice</a>";
                    bodyMessage = EmailHelperBase.GetMailBody(mailText.InvoiceDeliveryMailText, link);
                    break;
                default:
                    break;
            }

            var result = EmailHelper.SendMail(email, title, htmlTemplate.Replace("|MailBody|", bodyMessage));
            return result;
        }
    }
}