using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class ProductUnit : BaseEntityInt
    {
        [Required]
        public string Name { get; set; }
    }
}