using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.OrderHourService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class OrderHourController : Controller
    {
        private readonly IOrderHourService _orderHourService;
        public OrderHourController(IOrderHourService orderHourService)
        {
            _orderHourService = orderHourService;
        }

        [HttpGet]
        public IActionResult GetAllOrderHour()
        {
            try
            {
                var result = _orderHourService.GetAllOrderHour();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetOrderHourById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _orderHourService.GetOrderHourById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrderHour(OrderHourDto orderHourDto)
        {
            try
            {
                var result = _orderHourService.AddOrderHour(orderHourDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderHour(OrderHourDto orderHourDto)
        {
            try
            {
                var result = _orderHourService.UpdateOrderHour(orderHourDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteOrderHour(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _orderHourService.DeleteOrderHour(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}