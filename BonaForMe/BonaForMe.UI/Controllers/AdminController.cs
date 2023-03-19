using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.AdminService;
using BonaForMe.ServiceCore.ReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public IActionResult Reports()
        {
            return View();
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

        public FileResult CreatePDFReport(ReportDateDto reportDateDto)
        {
            var result = _reportService.ReportValue(reportDateDto).Data;

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
                style.ParagraphFormat.AddTabStop("18cm", TabAlignment.Right);

                style = document.Styles.AddStyle("Content3", "Normal");
                style.ParagraphFormat.SpaceAfter = 15;
                style.Font.Color = Colors.Black;
                style.ParagraphFormat.AddTabStop("10cm", TabAlignment.Right);
                style.ParagraphFormat.AddTabStop("18cm", TabAlignment.Right);

                style = document.Styles.AddStyle("PageTitle", "Normal");
                style.ParagraphFormat.SpaceAfter = 20;
                style.Font.Color = Colors.Black;
                style.Font.Size = 14;

                var culture = new CultureInfo("en");

                section.Add(CreatePageTitle("SOLMAZ PACKAGING LIMITED"));
                section.Add(Add2Columns("Merchant-ID MCRP24NZ", ((DateTime)reportDateDto.StartDate).ToString("dd MMMM yyyy HH:mm", culture) + "->" + ((DateTime)reportDateDto.EndDate).ToString("dd MMMM yyyy HH:mm", culture)));
                section.Add(Add2Columns("VAT-ID     3933414LH", ""));

                RevenueTaxSummary(section, result.RevenueTaxSummary);
                Taxes(section, result.Taxes);
                RevenueByCategory(section, result.RevenueByCategories);
                RevenueByEmployee(section, result.RevenueByEmployees);
                PaymentMethods(section, result.PaymentMethods);

                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();
                renderer.PdfDocument.Save(stream, true);

                var fileName = reportDateDto.StartDate.ToString() + " - " + reportDateDto.EndDate.ToString();
                return File(stream.ToArray(), "application/octet-stream", fileName  + ".pdf");
            }
        }

        private void RevenueTaxSummary(Section section, ReportColumnDto revenueTaxSummary)
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
            section.Add(Add3Columns(revenueTaxSummary.FirstColumn, revenueTaxSummary.SecondColumn, revenueTaxSummary.ThirdColumn));
        }

        private void Taxes(Section section, List<ReportColumnDto> taxes)
        {
            section.Add(CreateTitle("Taxes"));
            foreach (var tax in taxes)
            {
                section.Add(Add2Columns(tax.FirstColumn, tax.SecondColumn));
            }
        }

        private void RevenueByCategory(Section section, List<ReportColumnDto> revenueByCategories)
        {
            section.Add(CreateTitle("Revenue by category (gross)"));
            foreach (var category in revenueByCategories)
            {
                section.Add(Add3Columns(category.FirstColumn, category.SecondColumn, category.ThirdColumn));
            }
        }

        private void RevenueByEmployee(Section section, List<ReportColumnDto> revenueByEmployees)
        {
            section.Add(CreateTitle("Revenue by employee"));
            foreach (var employee in revenueByEmployees)
            {
                section.Add(Add3Columns(employee.FirstColumn, employee.SecondColumn, employee.ThirdColumn));
            }
        }

        private void PaymentMethods(Section section, List<ReportColumnDto> paymentMethods)
        {
            section.Add(CreateTitle("Sales revenue per payment method (gross)"));
            foreach (var method in paymentMethods)
            {
                section.Add(Add3Columns(method.FirstColumn, method.SecondColumn, method.ThirdColumn));
            }
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

        private Paragraph Add2Columns(string text1, string text2)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Style = "Content";
            paragraph.AddText(text1);
            paragraph.AddTab();
            paragraph.AddText(text2);
            return paragraph;
        }

        private Paragraph Add3Columns(string text1, string text2, string text3)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Style = "Content3";
            paragraph.AddText(text1);
            paragraph.AddTab();
            paragraph.AddText(text2);
            paragraph.AddTab();
            paragraph.AddText(text3);
            return paragraph;
        }
    }
}