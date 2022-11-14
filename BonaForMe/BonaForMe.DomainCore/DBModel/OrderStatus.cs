using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class OrderStatus : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string ColorCode { get; set; }
    }
}