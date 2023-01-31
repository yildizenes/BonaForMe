using System;

namespace BonaForMe.DomainCore.DTO
{
    public class OrderLogDto : DtoBaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}
