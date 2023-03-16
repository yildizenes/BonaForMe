using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.ApplicationSettingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class ApplicationSettingController : Controller
    {
        private readonly IApplicationSettingService _applicationSettingService;
        public ApplicationSettingController(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
        }

        [HttpGet]
        public IActionResult GetAllApplicationSetting()
        {
            try
            {
                var result = _applicationSettingService.GetAllApplicationSetting();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetApplicationSetting()
        {
            try
            {
                var result = _applicationSettingService.GetApplicationSetting();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddApplicationSetting(ApplicationSettingDto applicationSettingDto)
        {
            try
            {
                var result = _applicationSettingService.AddApplicationSetting(applicationSettingDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateApplicationSetting(ApplicationSettingDto applicationSettingDto)
        {
            try
            {
                var result = _applicationSettingService.UpdateApplicationSetting(applicationSettingDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteApplicationSetting(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _applicationSettingService.DeleteApplicationSetting(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpGet]
        public IActionResult GetMinimumOrderValue()
        {
            try
            {
                var result = _applicationSettingService.GetApplicationSetting();
                return Json(new { success = result.Success, data = result.Data.MinimumOrderValue, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }
    }
}