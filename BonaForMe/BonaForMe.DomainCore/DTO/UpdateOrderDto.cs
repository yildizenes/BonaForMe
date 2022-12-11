using System;

namespace BonaForMe.DomainCore.DTO
{
    public class UpdateOrderDto
    {
        public Guid OrderId { get; set; }
        public int OrderStatusId { get; set; }
    }
}
