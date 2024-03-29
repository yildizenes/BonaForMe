﻿using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult CheckStock(List<CheckStockDTO> checkStocks)
        {
            try
            {
                var result = _orderService.CheckStock(checkStocks);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CheckStockForFav(List<Guid> productIdList)
        {
            try
            {
                var result = _orderService.CheckStockForFav(productIdList);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(UpdateOrderDto updateOrderDto)
        {
            try
            {
                if (updateOrderDto.OrderId == Guid.Empty || updateOrderDto.OrderStatusId == 0)
                {
                    return Json(new { success = false, data = "", message = "Please enter a valid parameter." });
                }
                var result = _orderService.UpdateOrderStatus(updateOrderDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetUserOrderDetail(Guid userId)
        {
            try
            {
                var result = _orderService.GetUserOrderDetail(userId);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllOrder()
        {
            try
            {
                var result = _orderService.GetAllOrder();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllOrderByStatusId(int statusId)
        {
            try
            {
                var result = _orderService.GetAllOrderByStatusId(new List<int> { statusId });
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetUserNowOrderDetail(Guid userId)
        {
            try
            {
                var result = _orderService.GetAllOrderByStatusId(new List<int> { 1, 2, 3 }, userId);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetUserLastOrderDetail(Guid userId)
        {
            try
            {
                var result = _orderService.GetAllOrderByStatusId(new List<int> { 4, 5, 6, 7 }, userId);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetOrderById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _orderService.GetOrderById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrder(OrderDto orderDto)
        {
            try
            {
                var result = _orderService.AddOrder(orderDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateOrder(OrderDto orderDto)
        {
            try
            {
                var result = _orderService.UpdateOrder(orderDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteOrder(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _orderService.DeleteOrder(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}