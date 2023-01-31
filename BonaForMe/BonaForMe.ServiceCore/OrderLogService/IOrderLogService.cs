using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.OrderLogService
{
    public interface IOrderLogService
    {
        Result<OrderLogDto> AddOrderLog(OrderLogDto orderLogDto);
        Result DeleteOrderLog(Guid id);
        Result<OrderLogDto> UpdateOrderLog(OrderLogDto orderLogDto);
        Result<OrderLogDto> GetOrderLogById(Guid id);
        Result<List<OrderLogDto>> GetAllOrderLog();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
