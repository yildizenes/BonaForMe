using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class OrderDetailDto : DtoBaseEntity
    {
        public Guid OrderId { get; set; }

        public virtual Order Order { get; set; }

        public string PayType { get; set; }

        public decimal Price { get; set; }

        public string DeliveredName { get; set; }

        public string Phone { get; set; }

        public byte[] Signature { get; set; }

        public Guid CourierId { get; set; }

        public virtual User Courier { get; set; }
    }
}