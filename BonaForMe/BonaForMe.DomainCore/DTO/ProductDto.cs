using BonaForMe.DomainCore.DBModel;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class ProductDto : DtoBaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public Guid ProductUnitId { get; set; }

        public string ProductUnitName { get; set; }

        public virtual ProductUnit ProductUnit { get; set; }

        public decimal Price { get; set; }

        public Guid CurrencyUnitId { get; set; }

        public string CurrencyUnitName { get; set; }

        public virtual CurrencyUnit CurrencyUnit { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public virtual Category Category { get; set; }
    }
}