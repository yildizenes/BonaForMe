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
            string bodyMessage = "";

            switch (mailTypes)
            {
                case MailTypes.Welcome:
                    title = "Welcome to Bona Me For Me!";
                    bodyMessage = EmailHelperBase.GetMailBody(mailText.WelcomeMailText, notification);
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
                    var address = "https://www.bonameforme.com/Invoice/" + notification + ".pdf";
                    var link = "<a href='" + address + "'>View Invoice</a>";
                    bodyMessage = EmailHelperBase.GetMailBody(mailText.InvoiceDeliveryMailText, link);
                    break;
                default:
                    break;
            }

            var result = EmailHelper.SendMail(email, title, bodyMessage);
            return result;
        }
    }
}