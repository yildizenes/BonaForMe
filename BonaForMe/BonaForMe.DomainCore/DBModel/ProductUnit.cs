using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class ProductUnit : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}