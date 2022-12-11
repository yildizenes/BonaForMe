using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.OrderService
{
    public interface IOrderService
    {
        Result<OrderDto> AddOrder(OrderDto orderDto);
        Result DeleteOrder(Guid id);
        Result<OrderDto> UpdateOrder(OrderDto orderDto);
        Result<OrderDto> GetOrderById(Guid id);
        Result<List<OrderDto>> GetAllOrder();
        Result<List<OrderDto>> GetUserOrderDetail(Guid userId);
        Result<OrderDto> UpdateOrderStatus(UpdateOrderDto updateOrderDto);
        JsonResult FillDataTable(DataTableDto dataTable, int OrderStatusId);
    }
}
