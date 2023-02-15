using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.HomeService;
using BonaForMe.ServiceCore.ReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace BonaForMe.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IReportService _reportService;
        public HomeController(IHomeService homeService, IReportService reportService)
        {
            _homeService = homeService;
            _reportService = reportService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Dashboard()
        {
            var result = _homeService.GetDashboardData();
            var successInfo = TempData["Success"];
            if (successInfo != null)
            {
                ViewBag.Success = successInfo;
            }
            return View(result.Data);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetSalesValue(ReportDateDto reportDateDto)
        {
            try
            {
                var result = _reportService.GetSalesValue(reportDateDto);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FileResult CreateReport(string param)
        {
            var dates = Array.ConvertAll(param.Split(" - "), item => DateTime.Parse(item));
            var result = _reportService.CreateReport(new ReportDateDto { StartDate = dates[0], EndDate = dates[1] });

            return File(result.Data, "application/octet-stream", param + ".xlsx");
        }
    }
}
