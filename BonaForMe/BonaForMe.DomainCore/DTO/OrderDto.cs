using BonaForMe.DomainCore.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DTO
{
    public class OrderDto : DtoBaseEntity
    {
        public string OrderCode { get; set; }

        public string PayType { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public virtual User User { get; set; }

        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public Guid OrderHourId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual OrderHour OrderHour { get; set; }

        [NotMapped]
        public virtual List<LinkOrderProductDto> ProductList { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}