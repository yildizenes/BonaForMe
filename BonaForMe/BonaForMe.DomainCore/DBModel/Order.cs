using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class Order : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string OrderCode { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public Guid OrderStatusId { get; set; }

        [ForeignKey(nameof(OrderStatusId))]
        public virtual OrderStatus OrderStatus { get; set; }
    }
}