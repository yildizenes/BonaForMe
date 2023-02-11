using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class LinkOrderProductDto : DtoBaseEntity
    {
        public Guid OrderId { get; set; }

        public virtual Order Order { get; set; }

        public Guid ProductId { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }

        public bool IsCampaignProduct { get; set; }

        public virtual Product Product { get; set; }
    }
}