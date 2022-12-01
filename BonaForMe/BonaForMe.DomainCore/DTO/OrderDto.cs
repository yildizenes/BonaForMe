using BonaForMe.DomainCore.DBModel;
using System;
using System.Collections.Generic;

namespace BonaForMe.DomainCore.DTO
{
    public class OrderDto : DtoBaseEntity
    {
        public string OrderCode { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public virtual User User { get; set; }

        public Guid OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }

        public virtual List<LinkOrderProductDto> ProductList { get; set; }
    }
}