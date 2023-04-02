namespace BonaForMe.DomainCore.DTO
{
    public class UserDto : DtoBaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return this.FirstName + " " + this.LastName; } }

        public string UserMail { get; set; }

        public string UserPhone { get; set; }

        public string UserPassword { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Address { get; set; }

        public string CompanyName { get; set; }

        public string AirCode { get; set; }

        public string VATNo { get; set; }

        public bool IsApproved { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsCourier { get; set; }
    }
}