
namespace BonaForMe.DomainCore.DTO
{
    public class MailSenderDto : DtoBaseEntity
    {
        public string ToMailAddress { get; set; }
        public int MailTypes { get; set; }
        public string Notification { get; set; }
    }
}