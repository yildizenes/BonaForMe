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

                Style titleStyle = document.Styles.AddStyle("Title", "Normal");
                titleStyle.Font.Size = 12;
                titleStyle.Font.Color = Colors.LightGray;
                titleStyle.ParagraphFormat.SpaceBefore = 50;
                titleStyle.ParagraphFormat.SpaceAfter = 5;

                style = document.Styles.AddStyle("Content", "Normal");
                style.ParagraphFormat.SpaceAfter = 15;
                style.Font.Color = Colors.Black;

                style = document.Styles.AddStyle("PageTitle", "Normal");
                style.ParagraphFormat.SpaceAfter = 20;
                style.Font.Color = Colors.Black;
                style.Font.Size = 14;


                section.Add(CreatePageTitle("SOLMAZ PACKAGING LIMITED"));
                section.Add(Add2Columns("Merchant-ID MCRP24NZ", 55, "4 Feb 2023 00:00 -> 10 Feb 2023 23:45"));
                section.Add(Add2Columns("VAT-ID     3933414LH", 50, ""));

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
            paragraph.Format.Borders.DistanceFromTop = 3;
            section.Add(Add3Columns("Revenue gross", 82, "32", 25, "EUR13,455.10"));
        }

        private void Taxes(Section section)
        {
            section.Add(CreateTitle("Taxes"));
            section.Add(Add2Columns("Net", 126, "EUR10,939.08"));
            section.Add(Add2Columns("VAT 23%", 118, "EUR10,939.08"));
            section.Add(Add2Columns("Revenue gross ", 109, "EUR10,939.08"));
            section.Add(Add2Columns("Net total", 118, "EUR10,939.08"));
            section.Add(Add2Columns("Tax total", 118, "EUR10,939.08"));
            section.Add(Add2Columns("Revenue", 118, "EUR10,939.08"));
        }

        private void RevenueByCategory(Section section)
        {
            section.Add(CreateTitle("Revenue by category (gross)"));
            section.Add(Add2Columns("My Shelf", 118, "EUR13,455.10"));
        }

        private void RevenueByEmployee(Section section)
        {
            section.Add(CreateTitle("Revenue by employee"));
            section.Add(Add3Columns("solmazpackaging@gmail.com", 60, "32", 25, "EUR13,455.10"));
            section.Add(Add3Columns("Total", 95, "32", 25, "EUR13,455.10"));
        }

        private void SalesRevenue(Section section)
        {
            section.Add(CreateTitle("Sales revenue per payment method (gross)"));
            section.Add(Add3Columns("Cash", 95, "32", 25, "EUR13,455.10"));
            section.Add(Add3Columns("Total", 95, "32", 25, "EUR13,455.10"));
        }

        private Paragraph CreatePageTitle(string text)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Style = "PageTitle";
            paragraph.AddFormattedText(text, TextFormat.Bold);
            return paragraph;
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

        private Paragraph Add2Columns(string text1, int space, string text2)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Style = "Content";
            paragraph.AddText(text1);
            paragraph.AddSpace(space);
            paragraph.AddText(text2);
            return paragraph;
        }

        private Paragraph Add3Columns(string text1, int space1, string text2, int space2, string text3)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Style = "Content";
            paragraph.AddText(text1);
            paragraph.AddSpace(space1);
            paragraph.AddText(text2);
            paragraph.AddSpace(space2);
            paragraph.AddText(text3);
            return paragraph;
        }
    }
}