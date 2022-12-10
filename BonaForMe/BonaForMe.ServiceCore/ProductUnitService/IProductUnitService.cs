using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.ProductUnitService
{
    public interface IProductUnitService
    {
        Result<ProductUnitDto> AddProductUnit(ProductUnitDto productUnitDto);
        Result DeleteProductUnit(int id);
        Result<ProductUnitDto> UpdateProductUnit(ProductUnitDto productUnitDto);
        Result<ProductUnitDto> GetProductUnitById(int id);
        Result<List<ProductUnitDto>> GetAllProductUnit();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
