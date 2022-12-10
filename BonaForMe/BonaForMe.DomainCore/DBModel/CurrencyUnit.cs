using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class CurrencyUnit : BaseEntityInt
    {
        [Required]
        public string Name { get; set; }
    }
}