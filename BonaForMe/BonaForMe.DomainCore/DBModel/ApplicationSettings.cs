using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class ApplicationSettings : BaseEntity
    {
        [MaxLength()]
        public string AboutUs { get; set; }

        [MaxLength()]
        public string Vision { get; set; }

        [MaxLength()]
        public string Mission { get; set; }

        [MaxLength()]
        public string Address { get; set; }

        [MaxLength()]
        public string InfoMail { get; set; }

        [MaxLength()]
        public string Telephone { get; set; }
        
        [MaxLength()]
        public string LinkedIn { get; set; }

        [MaxLength()]
        public string Twitter { get; set; }

        [MaxLength()]
        public string Instagram { get; set; }

        [MaxLength()]
        public string Facebook { get; set; }

        [MaxLength()]
        public string PlayStoreAddress { get; set; }

        [MaxLength()]
        public string AppleStoreAddress { get; set; }

        [MaxLength()]
        public string HuaweiStoreAddress { get; set; }
    }
}