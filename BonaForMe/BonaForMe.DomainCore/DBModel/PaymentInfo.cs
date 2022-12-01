using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class PaymentInfo : BaseEntity
    {
        [Required]
        public string Description { get; set; }
    }
}