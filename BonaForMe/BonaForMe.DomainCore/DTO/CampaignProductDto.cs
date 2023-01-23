using System;

namespace BonaForMe.DomainCore.DTO
{
    public class CampaignProductDto : DtoBaseEntity
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
        public int? GroupId { get; set; }
    }
}