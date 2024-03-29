﻿using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class ApplicationSetting : BaseEntity
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
        public string GoogleMaps { get; set; }

        [MaxLength()]
        public string InfoMail { get; set; }

        [MaxLength()]
        public string Telephone { get; set; }

        [MaxLength()]
        public string OpenCloseTime { get; set; }

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

        [MaxLength()]
        public int MinimumOrderValue { get; set; }

        [MaxLength()]
        public string WelcomeMailText { get; set; }

        [MaxLength()]
        public string ForgetPasswordMailText { get; set; }

        [MaxLength()]
        public string MobileApproveMailText { get; set; }

        [MaxLength()]
        public string InvoiceDeliveryMailText { get; set; }
    }
}