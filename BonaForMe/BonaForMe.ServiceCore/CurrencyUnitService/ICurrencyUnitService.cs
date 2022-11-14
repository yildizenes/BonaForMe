using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.CurrencyUnitService
{
    public interface ICurrencyUnitService
    {
        Result<CurrencyUnitDto> AddCurrencyUnit(CurrencyUnitDto currencyUnitDto);
        Result DeleteCurrencyUnit(Guid id);
        Result<CurrencyUnitDto> UpdateCurrencyUnit(CurrencyUnitDto currencyUnitDto);
        Result<CurrencyUnitDto> GetCurrencyUnitById(Guid id);
        Result<List<CurrencyUnitDto>> GetAllCurrencyUnit();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
