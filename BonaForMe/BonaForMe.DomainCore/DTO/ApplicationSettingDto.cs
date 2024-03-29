﻿namespace BonaForMe.DomainCore.DTO
{
    public class ApplicationSettingDto : DtoBaseEntity
    {
        public string AboutUs { get; set; }
        public string Vision { get; set; }
        public string Mission { get; set; }
        public string Address { get; set; }
        public string GoogleMaps { get; set; }
        public string InfoMail { get; set; }
        public string Telephone { get; set; }
        public string OpenCloseTime { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string PlayStoreAddress { get; set; }
        public string AppleStoreAddress { get; set; }
        public string HuaweiStoreAddress { get; set; }
        public int MinimumOrderValue { get; set; }
        public string WelcomeMailText { get; set; }
        public string ForgetPasswordMailText { get; set; }
        public string MobileApproveMailText { get; set; }
        public string InvoiceDeliveryMailText { get; set; }
    }
}