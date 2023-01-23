using BonaForMe.DomainCore.DBModel;
using Microsoft.AspNetCore.Http;
using System;

namespace BonaForMe.DomainCore.DTO
{
    public class ProductDto : DtoBaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int ProductUnitId { get; set; }

        public string ProductUnitName { get; set; }

        public virtual ProductUnit ProductUnit { get; set; }

        public int TaxRate { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }

        public int CurrencyUnitId { get; set; }

        public string CurrencyUnitName { get; set; }

        public virtual CurrencyUnit CurrencyUnit { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public virtual Category Category { get; set; }

        public IFormFile FormFile { get; set; }
    }
}