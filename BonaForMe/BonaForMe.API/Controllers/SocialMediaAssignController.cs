using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BonaForMe.DomainCommonCore.Enums;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.SocialMediaAssignService;
using BonaForMe.ServicesCore.LogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class SocialMediaAssignController : Controller
    {
        private readonly ISocialMediaAssignService _socialMediaAssignService;
        private readonly ILogService _logService;
        public SocialMediaAssignController(ISocialMediaAssignService socialMediaAssignService, ILogService logService)
        {
            _socialMediaAssignService = socialMediaAssignService;
            _logService = logService;
        }
        [AllowAnonymous]
        [HttpGet("{mmCardNo}")]
        public IActionResult GetAllSocialMediaAssignList(Guid mmCardNo)
        {
            try
            {
                if (mmCardNo == Guid.Empty)
                {
                    return Json(new { Success = false, Data= "", Message= "Request Paramaters are not found" });
                }

                var socialMediaAssignListInfo = _socialMediaAssignService.GetAllSocialMediaAssignByBonaForMeNo(mmCardNo);
                if (socialMediaAssignListInfo != null && socialMediaAssignListInfo.Count > 0)
                {
                    return Json(new { success = true, data = socialMediaAssignListInfo, message = "Process is Success" });
                }
                else
                {
                    return Json(new { success = false, data = "", message = "Social Media Assign List is empty" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddSocialMediaAssign(SocialMediaAssignDTO socialMediaAssignDTO)
        {
      
            try
            {
                Result<SocialMediaAssign> result = new Result<SocialMediaAssign>();
                result = _socialMediaAssignService.AddSocialMediaAssign(socialMediaAssignDTO);
                if (result.Success != false)
                {
                    _logService.SaveLog(result.Data.Id, "SOCIALMEDIAASSIGN.ID", "AddSocialMediaAssign Add Process is Success", AppEnums.LogTypes.Success, "SocialMediaAssignController.AddSocialMediaAssign", "", socialMediaAssignDTO.UserId);
                    return Json(new { success = true, data = result.Data, message = "Process is Success" });
                }
                else
                {
                    _logService.SaveLog(Guid.Empty, "SOCIALMEDIAASSIGN.ID", "AddSocialMediaAssign Add Process is Failed", AppEnums.LogTypes.UnSuccess, "SocialMediaAssignController.AddSocialMediaAssign", "", socialMediaAssignDTO.UserId, "");
                    return Json(new { success = false, data = "", message = "Process is Failed" });
                }

            }
            catch (Exception ex)
            {
                _logService.SaveLog(Guid.Empty, "SOCIALMEDIAASSIGN.ID", "Social Media Assign Add Process is Error", AppEnums.LogTypes.Error, "SocialMediaAssignController.AddSocialMediaAssign", "", socialMediaAssignDTO.UserId, ex.Message, true);
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EditSocialMediaAssign(SocialMediaAssignDTO socialMediaAssignDTO)
        {

            try
            {
                Result<SocialMediaAssign> result = new Result<SocialMediaAssign>();
                result = _socialMediaAssignService.UpdateSocialMediaAssign(socialMediaAssignDTO);
                if (result.Success != false)
                {
                    _logService.SaveLog(socialMediaAssignDTO.Id, "SOCIALMEDIAASSIGN.ID", "AddSocialMediaAssign Edit Process is Success", AppEnums.LogTypes.Success, "SocialMediaAssignController.EditSocialMediaAssign", "", socialMediaAssignDTO.UserId);
                    return Json(new { success = true, data = result.Data, message = "Process is Success" });
                }
                else
                {
                    _logService.SaveLog(socialMediaAssignDTO.Id, "SOCIALMEDIAASSIGN.ID", "AddSocialMediaAssign Edit Process is Failed", AppEnums.LogTypes.UnSuccess, "SocialMediaAssignController.EditSocialMediaAssign", "", socialMediaAssignDTO.UserId, result.Message);
                    return Json(new { success = false, data = "", message = "Process is Failed" });
                }

            }
            catch (Exception ex)
            {
                _logService.SaveLog(socialMediaAssignDTO.Id, "SOCIALMEDIAASSIGN.ID", "Social Media Assign Edit Process is Error", AppEnums.LogTypes.Error, "SocialMediaAssignController.EditSocialMediaAssign", "", socialMediaAssignDTO.UserId, ex.Message, true);
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost("{socialMediaAssignId}")]
        public IActionResult DeleteSocialMediaAssign(Guid socialMediaAssignId)
        {

            try
            {
                Result<SocialMediaAssign> result = new Result<SocialMediaAssign>();
                result = _socialMediaAssignService.DeleteSocialMediaAssign(socialMediaAssignId);
                if (result.Success != false)
                {
                    return Json(new { success = true, data = result.Data, message = "Process is Success" });
                }
                else
                {
                    return Json(new { success = false, data = "", message = "Process is Failed" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }
    }
}
