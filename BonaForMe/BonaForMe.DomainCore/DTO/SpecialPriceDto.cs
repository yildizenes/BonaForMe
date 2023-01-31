using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class SpecialPriceDto : DtoBaseEntity
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public virtual User User { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public virtual Product Product { get; set; }

        public decimal Price { get; set; }
    }
}