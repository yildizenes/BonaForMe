using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class CampaignProductDto : DtoBaseEntity
    {
        public Guid ProductId { get; set; }

        public int Amount { get; set; }

        public decimal? TopPriceLimit { get; set; }

        public string ProductName { get; internal set; }

        public virtual Product Product { get; set; }
    }
}