using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.OrderStatusService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class OrderStatusController : Controller
    {
        private readonly IOrderStatusService _orderStatusService;
        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            _orderStatusService = orderStatusService;
        }

        [HttpGet]
        public IActionResult GetAllOrderStatus()
        {
            try
            {
                var result = _orderStatusService.GetAllOrderStatus();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetOrderStatusById(int id)
        {
            try
            {
                if (id == 0)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _orderStatusService.GetOrderStatusById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrderStatus(OrderStatusDto orderStatusDto)
        {
            try
            {
                var result = _orderStatusService.AddOrderStatus(orderStatusDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(OrderStatusDto orderStatusDto)
        {
            try
            {
                var result = _orderStatusService.UpdateOrderStatus(orderStatusDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteOrderStatus(int id)
        {
            try
            {
                if (id == 0)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _orderStatusService.DeleteOrderStatus(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}