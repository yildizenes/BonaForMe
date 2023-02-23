using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.OrderHourService
{
    public interface IOrderHourService
    {
        Result<OrderHourDto> AddOrderHour(OrderHourDto orderHourDto);
        Result DeleteOrderHour(Guid id);
        Result<OrderHourDto> UpdateOrderHour(OrderHourDto orderHourDto);
        Result<OrderHourDto> GetOrderHourById(Guid id);
        Result<List<OrderHourDto>> GetAllOrderHour();
        JsonResult FillDataTable(DataTableDto dataTable);

    }
}
