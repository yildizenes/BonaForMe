using System.ComponentModel.DataAnnotations;

namespace BonaForMe.DomainCore.DBModel
{
    public class ContactInformation : BaseEntity
    {
        [MaxLength(20)]
        public string WhattsappPhone { get; set; }

        [MaxLength(100)]
        public string EmailInfo { get; set; }

        [MaxLength(20)]
        public string CustomerSupportPhone { get; set; }
    }
}