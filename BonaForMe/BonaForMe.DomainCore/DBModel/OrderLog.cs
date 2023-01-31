using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class OrderLog : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}