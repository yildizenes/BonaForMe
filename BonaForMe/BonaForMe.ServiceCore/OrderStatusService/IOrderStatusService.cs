using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.OrderStatusService
{
    public interface IOrderStatusService
    {
        Result<OrderStatusDto> AddOrderStatus(OrderStatusDto orderStatusDto);
        Result DeleteOrderStatus(Guid id);
        Result<OrderStatusDto> UpdateOrderStatus(OrderStatusDto orderStatusDto);
        Result<OrderStatusDto> GetOrderStatusById(Guid id);
        Result<List<OrderStatusDto>> GetAllOrderStatus();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
