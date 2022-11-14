using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.ProductService
{
    public interface IProductService
    {
        Result<ProductDto> AddProduct(ProductDto productDto);
        Result DeleteProduct(Guid id);
        Result<ProductDto> UpdateProduct(ProductDto productDto);
        Result<ProductDto> GetProductById(Guid id);
        Result<List<ProductDto>> GetAllProduct();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
