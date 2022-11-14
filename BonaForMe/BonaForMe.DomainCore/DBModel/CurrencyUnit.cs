using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class CurrencyUnit : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}