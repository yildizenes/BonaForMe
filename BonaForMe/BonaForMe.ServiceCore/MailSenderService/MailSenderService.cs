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

        public Result SendMail(string email, MailTypes mailTypes, string notification)
        {
            Result result = new Result();
            EmailHelper.SendMail(email, mailTypes, notification);

            result.Success = true;
            return result;
        }
    }
}