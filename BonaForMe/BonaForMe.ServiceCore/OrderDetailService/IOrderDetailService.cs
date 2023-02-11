using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.OrderDetailService
{
    public interface IOrderDetailService
    {
        Result<OrderDetailDto> AddOrderDetail(OrderDetailDto orderDetailDto);
        Result DeleteOrderDetail(Guid id);
        Result<OrderDetailDto> UpdateOrderDetail(OrderDetailDto orderDetailDto);
        Result<OrderDetailDto> GetOrderDetailById(Guid id);
        Result<List<OrderDetailDto>> GetAllOrderDetail();
        JsonResult FillDataTable(DataTableDto dataTable);
    }
}
