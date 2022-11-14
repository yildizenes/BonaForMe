using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.ServiceCore.SocialMediaListService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class SocialMediaListController : Controller
    {
        private readonly ISocialMediaListService _socialMediaListService;
        public SocialMediaListController(ISocialMediaListService socialMediaListService)
        {
            _socialMediaListService = socialMediaListService;
        }
        [HttpGet]
        public IActionResult GetAllSocialMediaList()
        {
            try
            {
                var socialMediaListInfo = _socialMediaListService.GetAllSocialMediaLists();
                if (socialMediaListInfo != null && socialMediaListInfo.Count > 0)
                {
                    return Json(new { success = true, data = socialMediaListInfo, message = "Process is Success" });
                }
                else
                {
                    return Json(new { success = false, data = "", message = "Social Media List is empty" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }
    }
}
