using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.SpecialPriceService
{
    public interface ISpecialPriceService
    {
        Result<SpecialPriceDto> AddSpecialPrice(SpecialPriceDto specialPriceDto);
        Result DeleteSpecialPrice(Guid id);
        Result<SpecialPriceDto> UpdateSpecialPrice(SpecialPriceDto specialPriceDto);
        Result<SpecialPriceDto> GetSpecialPriceById(Guid id);
        Result<List<SpecialPriceDto>> GetSpecialPriceByUserId(Guid id);
        Result<List<SpecialPriceDto>> GetSpecialPriceByFilters(Guid userId, Guid categoryId);
        Result<List<SpecialPriceDto>> GetAllSpecialPrice();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
