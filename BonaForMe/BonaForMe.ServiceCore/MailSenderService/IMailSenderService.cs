using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;

namespace BonaForMe.ServiceCore.MailSenderService
{
    public interface IMailSenderService
    {
        Result SendMail(string Email, string newPassword);
    }
}
