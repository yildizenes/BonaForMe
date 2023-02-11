using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class LinkOrderProduct : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Count { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        public bool IsCampaignProduct { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}