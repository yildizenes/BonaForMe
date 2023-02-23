using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.WorkPartnerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class WorkPartnerController : Controller
    {
        private readonly IWorkPartnerService _workPartnerService;
        public WorkPartnerController(IWorkPartnerService workPartnerService)
        {
            _workPartnerService = workPartnerService;
        }

        [HttpGet]
        public IActionResult GetAllWorkPartner()
        {
            try
            {
                var result = _workPartnerService.GetAllWorkPartner();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetWorkPartnerById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _workPartnerService.GetWorkPartnerById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddWorkPartner(WorkPartnerDto workPartnerDto)
        {
            try
            {
                var result = _workPartnerService.AddWorkPartner(workPartnerDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateWorkPartner(WorkPartnerDto workPartnerDto)
        {
            try
            {
                var result = _workPartnerService.UpdateWorkPartner(workPartnerDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteWorkPartner(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _workPartnerService.DeleteWorkPartner(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}