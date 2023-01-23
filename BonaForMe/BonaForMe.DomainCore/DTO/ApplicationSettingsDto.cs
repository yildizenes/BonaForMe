namespace BonaForMe.DomainCore.DTO
{
    public class ApplicationSettingsDto : DtoBaseEntity
    {
        public string AboutUs { get; set; }
        public string Vision { get; set; }
        public string Mission { get; set; }
        public string Address { get; set; }
        public string InfoMail { get; set; }
        public string Telephone { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string PlayStoreAddress { get; set; }
        public string AppleStoreAddress { get; set; }
        public string HuaweiStoreAddress { get; set; }
    }
}