namespace BonaForMe.DomainCore.DTO
{
    public class ContactInformationDto : DtoBaseEntity
    {
        public string WhattsappPhone { get; set; }

        public string EmailInfo { get; set; }

        public string CustomerSupportPhone { get; set; }
    }
}