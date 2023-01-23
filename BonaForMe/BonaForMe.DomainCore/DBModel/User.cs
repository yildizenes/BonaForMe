using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class User : BaseEntity
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get { return this.FirstName + " " + this.LastName; } }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserMail { get; set; }

        [MaxLength(20)]
        public string UserPhone { get; set; }

        [Required]
        public string UserPassword { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        public string Address { get; set; }

        [MaxLength(100)]
        public string CompanyName { get; set; }

        public bool IsApproved { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsCourier { get; set; }
    }
}