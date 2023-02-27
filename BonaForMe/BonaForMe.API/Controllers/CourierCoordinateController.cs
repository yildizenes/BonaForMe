using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.CourierCoordinateService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class CourierCoordinateController : Controller
    {
        private readonly ICourierCoordinateService _courierCoordinateService;
        public CourierCoordinateController(ICourierCoordinateService courierCoordinateService)
        {
            _courierCoordinateService = courierCoordinateService;
        }

        [HttpGet]
        public IActionResult GetAllCourierCoordinate()
        {
            try
            {
                var result = _courierCoordinateService.GetAllCourierCoordinate();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCourierCoordinateById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _courierCoordinateService.GetCourierCoordinateById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCourierCoordinateByCourierId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _courierCoordinateService.GetCourierCoordinateByCourierId(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddCourierCoordinate(CourierCoordinateDto courierCoordinateDto)
        {
            try
            {
                var result = _courierCoordinateService.AddCourierCoordinate(courierCoordinateDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateCourierCoordinate(CourierCoordinateDto courierCoordinateDto)
        {
            try
            {
                var result = _courierCoordinateService.UpdateCourierCoordinate(courierCoordinateDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCourierCoordinate(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _courierCoordinateService.DeleteCourierCoordinate(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}