using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.CampaignProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class CampaignProductController : Controller
    {
        private readonly ICampaignProductService _campaignProductService;
        public CampaignProductController(ICampaignProductService campaignProductService)
        {
            _campaignProductService = campaignProductService;
        }

        [HttpGet]
        public IActionResult GetAllCampaignProduct()
        {
            try
            {
                var result = _campaignProductService.GetAllCampaignProduct();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCampaignProductById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _campaignProductService.GetCampaignProductById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddCampaignProduct(CampaignProductDto campaignProductDto)
        {
            try
            {
                var result = _campaignProductService.AddCampaignProduct(campaignProductDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateCampaignProduct(CampaignProductDto campaignProductDto)
        {
            try
            {
                var result = _campaignProductService.UpdateCampaignProduct(campaignProductDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCampaignProduct(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _campaignProductService.DeleteCampaignProduct(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCampaignProductsByTopLimit(decimal topLimit)
        {
            try
            {
                var result = _campaignProductService.GetCampaignProductsByTopLimit(topLimit);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }
    }
}