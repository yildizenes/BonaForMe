using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class OrderDetail : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [Required]
        [MaxLength(100)]
        public string PayType { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string DeliveredName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public byte[] Signature { get; set; }

        [Required]
        public Guid CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public virtual User Courier { get; set; }
    }
}