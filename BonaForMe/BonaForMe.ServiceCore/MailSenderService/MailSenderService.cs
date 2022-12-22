using BonaForMe.DomainCommonCore.Helper;
using BonaForMe.DomainCommonCore.Result;
using Microsoft.AspNetCore.Hosting;

namespace BonaForMe.ServiceCore.MailSenderService
{
    public class MailSenderService : IMailSenderService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MailSenderService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Result SendMail(string email, string newPassword)
        {
            Result result = new Result();
            EmailHelper.SendForgetPasswordMail(email, newPassword, System.IO.File.ReadAllText($@"{_webHostEnvironment.WebRootPath}\MailTemplates\EmailTemplate.html"));

            result.Success = true;
            return result;
        }
    }
}