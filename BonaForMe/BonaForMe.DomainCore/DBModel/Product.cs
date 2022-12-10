using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonaForMe.DomainCore.DBModel
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; }

        [Required]
        public int ProductUnitId { get; set; }

        [ForeignKey(nameof(ProductUnitId))]
        public virtual ProductUnit ProductUnit { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        public int CurrencyUnitId { get; set; }

        [ForeignKey(nameof(CurrencyUnitId))]
        public virtual CurrencyUnit CurrencyUnit { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }
    }
}