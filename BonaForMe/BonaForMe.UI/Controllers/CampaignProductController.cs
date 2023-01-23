using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.CampaignProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BonaForMe.UI.Controllers
{
    [Authorize]
    public class CampaignProductController : Controller
    {
        private readonly ICampaignProductService _campaignProductService;
        public CampaignProductController(ICampaignProductService campaignProductService)
        {
            _campaignProductService = campaignProductService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(CampaignProductDto campaignProductDto)
        {
            try
            {
                var result = _campaignProductService.AddCampaignProduct(campaignProductDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Update(CampaignProductDto campaignProductDto)
        {
            try
            {
                var result = _campaignProductService.UpdateCampaignProduct(campaignProductDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var result = _campaignProductService.DeleteCampaignProduct(id);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult GetCampaignProductById(Guid id)
        {
            try
            {
                var result = _campaignProductService.GetCampaignProductById(id);
                if (result != null)
                {
                    return new JsonResult(result.Data);
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetAllCampaignProduct()
        {
            try
            {
                var result = _campaignProductService.GetAllCampaignProduct();
                return new JsonResult(result.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult LoadCampaignProductData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                //Paging Size (10,20,50,100)
                int pageSize = Convert.ToInt32(length) != -1 ? Convert.ToInt32(length) : 100;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                DataTableDto dataTable = new DataTableDto()
                {
                    Draw = draw,
                    PageSize = pageSize,
                    Skip = skip,
                    SearchValue = searchValue,
                    SortColumnDirection = sortColumnDirection,
                    SortColumn = sortColumn
                };
                return _campaignProductService.FillDataTable(dataTable);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}