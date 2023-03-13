using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.AdminService;
using BonaForMe.ServiceCore.ReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.IO;

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

        public FileResult CreateReport(ReportDateDto reportDateDto)
        {
            var result = _reportService.CreateReport(reportDateDto);
            var fileName = reportDateDto.StartDate.ToString() + " - " + reportDateDto.EndDate.ToString();
            return File(result.Data, "application/octet-stream", fileName + ".xlsx");
        }

        public FileResult CreatePDFReport(/*ReportDateDto reportDateDto*/)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document();
                document.DefaultPageSetup.RightMargin = 30;
                document.DefaultPageSetup.LeftMargin = 30;

                Section section = document.AddSection();

                Style style = document.Styles["Normal"];
                style.Font.Name = "Verdana";
                style.Font.Color = Colors.Gray;

                Style titleStyle = document.Styles.AddStyle("Title", "Normal");
                titleStyle.Font.Size = 12;
                titleStyle.Font.Color = Colors.LightGray;
                titleStyle.ParagraphFormat.SpaceAfter = 15;


                style = document.Styles.AddStyle("Reference", "Normal");
                style.ParagraphFormat.SpaceBefore = "5mm";
                style.ParagraphFormat.SpaceAfter = "5mm";
                style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);


                RevenueTaxSummary(section);
                Taxes(section);
                RevenueByCategory(section);
                RevenueByEmployee(section);
                SalesRevenue(section);

                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();
                renderer.PdfDocument.Save(stream, true);
                return File(stream.ToArray(), "application/octet-stream", "asdasd" + ".pdf");
            }
        }

        private void RevenueTaxSummary(Section section)
        {
            Paragraph paragraph = section.AddParagraph();
            paragraph.Style = "Title";
            paragraph.AddText("Revenue and tax summary");
            paragraph.AddSpace(20);
            paragraph.AddText("Number of transactions");
            paragraph.AddSpace(20);
            paragraph.AddText("Amount");

            paragraph.Format.Borders.Bottom = new Border { Width = 1, Color = Colors.LightGray };
            paragraph.Format.Borders.Distance = 3;
        }

        private void Taxes(Section section)
        {
            section.Add(CreateTitle("Taxes"));
            Paragraph paragraph = section.AddParagraph();
        }

        private void RevenueByCategory(Section section)
        {
            section.Add(CreateTitle("Revenue by category (gross)"));
            Paragraph paragraph = section.AddParagraph();
        }

        private void RevenueByEmployee(Section section)
        {
            section.Add(CreateTitle("Revenue by employee"));
            Paragraph paragraph = section.AddParagraph();
        }

        private void SalesRevenue(Section section)
        {
            section.Add(CreateTitle("Sales revenue per payment method (gross)"));

            Paragraph paragraph = section.AddParagraph();
        }

        private Paragraph CreateTitle(string text)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Style = "Title";
            paragraph.AddText(text);
            paragraph.Format.Borders.Bottom = new Border { Width = 1, Color = Colors.LightGray };
            paragraph.Format.Borders.Distance = 3;
            return paragraph;
        }
    }
}