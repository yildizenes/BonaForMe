using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class WorkPartner : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; }
    }
}