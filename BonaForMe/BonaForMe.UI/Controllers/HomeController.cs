using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.AdminService;
using BonaForMe.ServiceCore.ReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IReportService _reportService;
        public AdminController(IAdminService adminService, IReportService reportService)
        {
            _adminService = adminService;
            _reportService = reportService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Dashboard()
        {
            var result = _adminService.GetDashboardData();
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
            catch (Exception)
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
