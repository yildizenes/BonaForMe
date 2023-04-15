using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace BonaForMe.ServiceCore.ReportService
{
    public class ReportService : IReportService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;

        public ReportService(BonaForMeDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Result<ReportDto> GetSalesValue(ReportDateDto reportDateDto)
        {
            Result<ReportDto> result = new Result<ReportDto>();
            try
            {
                var data = _context.OrderLogs
                    .Include(x => x.Product)
                    .Include(x => x.Order).ThenInclude(x => x.User)
                    .Include(x => x.Order).ThenInclude(x => x.OrderStatus)
                    .Where(x => x.IsActive && !x.IsDeleted && x.Order.IsActive && !x.Order.IsDeleted).ToList();

                if (reportDateDto.StartDate != null)
                    data = data.Where(x => x.DateCreated >= reportDateDto.StartDate && x.DateCreated <= reportDateDto.EndDate).ToList();

                var totalPrice = data.Sum(x => x.Price);
                var totalTaxRate = data.Sum(x => (x.Price * x.Product.TaxRate) / 100);

                result.Data = new ReportDto { SalesValue = totalPrice, RevenueValue = totalTaxRate };
                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        public Result<byte[]> CreateReport(ReportDateDto reportDateDto)
        {
            Result<byte[]> result = new Result<byte[]>();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);

                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = new Sheets();
                    sheets.Append(new Sheet { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Report Value" });
                    workbookPart.Workbook.Append(sheets);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    #region Title

                    Row row1 = new Row() { RowIndex = 1 };
                    sheetData.Append(row1);

                    row1.Append(new Cell() { CellReference = "A1", DataType = CellValues.String, CellValue = new CellValue("Account") });
                    row1.Append(new Cell() { CellReference = "B1", DataType = CellValues.String, CellValue = new CellValue("Date") });
                    row1.Append(new Cell() { CellReference = "C1", DataType = CellValues.String, CellValue = new CellValue("Time") });
                    row1.Append(new Cell() { CellReference = "D1", DataType = CellValues.String, CellValue = new CellValue("Type") });
                    row1.Append(new Cell() { CellReference = "E1", DataType = CellValues.String, CellValue = new CellValue("Order Code") });
                    row1.Append(new Cell() { CellReference = "F1", DataType = CellValues.String, CellValue = new CellValue("Payment Method") });
                    row1.Append(new Cell() { CellReference = "G1", DataType = CellValues.String, CellValue = new CellValue("Quantity") });
                    row1.Append(new Cell() { CellReference = "H1", DataType = CellValues.String, CellValue = new CellValue("Description") });
                    row1.Append(new Cell() { CellReference = "I1", DataType = CellValues.String, CellValue = new CellValue("Currency") });
                    row1.Append(new Cell() { CellReference = "J1", DataType = CellValues.String, CellValue = new CellValue("Price(Gross)") });
                    row1.Append(new Cell() { CellReference = "K1", DataType = CellValues.String, CellValue = new CellValue("Price(Net)") });
                    row1.Append(new Cell() { CellReference = "L1", DataType = CellValues.String, CellValue = new CellValue("Tax") });
                    row1.Append(new Cell() { CellReference = "M1", DataType = CellValues.String, CellValue = new CellValue("Tax Rate") });

                    #endregion

                    #region Items

                    var orderLogs = _context.OrderLogs
                        .Include(x => x.Order).ThenInclude(x => x.User)
                        .Include(x => x.Product).ThenInclude(x => x.CurrencyUnit)
                        .Where(x => x.DateCreated >= reportDateDto.StartDate && x.DateCreated <= reportDateDto.EndDate
                                    && x.IsActive && !x.IsDeleted && x.Order.IsActive && !x.Order.IsDeleted).ToList();

                    int index = 2;
                    foreach (var orderLog in orderLogs)
                    {
                        Row row = new Row() { RowIndex = Convert.ToUInt32(index) };
                        sheetData.Append(row);

                        var taxPrice = (orderLog.Price * orderLog.Count * orderLog.Product.TaxRate) / 100;
                        var company = orderLog.Order.User.CompanyName == null || orderLog.Order.User.CompanyName == string.Empty ? orderLog.Order.User.FullName : orderLog.Order.User.CompanyName;
                        row.Append(new Cell() { CellReference = "A" + index, DataType = CellValues.String, CellValue = new CellValue(company) });
                        row.Append(new Cell() { CellReference = "B" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.DateCreated.ToShortDateString()) });
                        row.Append(new Cell() { CellReference = "C" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.DateCreated.ToShortTimeString()) });
                        row.Append(new Cell() { CellReference = "D" + index, DataType = CellValues.String, CellValue = new CellValue("Sales") });
                        row.Append(new Cell() { CellReference = "E" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.Order.OrderCode) });
                        row.Append(new Cell() { CellReference = "F" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.Order.PayType) });
                        row.Append(new Cell() { CellReference = "G" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.Count) });
                        row.Append(new Cell() { CellReference = "H" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.Product.Name) });
                        row.Append(new Cell() { CellReference = "I" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.Product.CurrencyUnit.Name) });
                        row.Append(new Cell() { CellReference = "J" + index, DataType = CellValues.String, CellValue = new CellValue(Math.Round((orderLog.Price * orderLog.Count) + taxPrice, 2)) });
                        row.Append(new Cell() { CellReference = "K" + index, DataType = CellValues.String, CellValue = new CellValue(Math.Round(orderLog.Price * orderLog.Count, 2)) });
                        row.Append(new Cell() { CellReference = "L" + index, DataType = CellValues.String, CellValue = new CellValue(Math.Round(taxPrice, 2)) });
                        row.Append(new Cell() { CellReference = "M" + index, DataType = CellValues.String, CellValue = new CellValue(orderLog.Product.TaxRate) });
                        index++;
                    }

                    #endregion

                    document.Close();
                    result.Data = ms.ToArray();
                }

                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }


        public Result<ReportDatasDto> ReportValue(ReportDateDto reportDateDto)
        {
            Result<ReportDatasDto> result = new Result<ReportDatasDto>();
            ReportDatasDto reportDatasDto = new ReportDatasDto();

            try
            {
                var data = _context.OrderLogs
                    .Include(x => x.Product).ThenInclude(x => x.Category)
                    .Include(x => x.Product).ThenInclude(x => x.CurrencyUnit)
                    .Include(x => x.Order).ThenInclude(x => x.User)
                    .Include(x => x.Order).ThenInclude(x => x.OrderStatus)
                    .Where(x => x.DateCreated >= reportDateDto.StartDate && x.DateCreated <= reportDateDto.EndDate && x.Price > 0
                        && x.IsActive && !x.IsDeleted && x.Order.IsActive && !x.Order.IsDeleted).ToList();

                var totalPrice = data.Sum(x => x.Count * x.Price);
                var totalTaxRate = data.Sum(x => x.Count * x.Price * x.Product.TaxRate / 100);
                var revenue = string.Format("{0:0.00}", totalPrice + totalTaxRate);
                //var revenue = data.Sum(item => item.Count * item.Price * (1 + (decimal)item.Product.TaxRate / 100));

                reportDatasDto.RevenueTaxSummary = new ReportColumnDto { FirstColumn = "Revenue Gross", SecondColumn = data.Count().ToString(), ThirdColumn = "EUR" + string.Format("{0:0.00}", revenue) };

                var taxRates = data.GroupBy(x => x.Product.TaxRate).OrderBy(x => x.Key).Select(g => new ReportColumnDto
                {
                    FirstColumn = "VAT " + g.Key + "%",
                    SecondColumn = "EUR" + string.Format("{0:0.00}", g.Sum(a => (a.Count * a.Price * a.Product.TaxRate) / 100))
                }); // TaxRate - Count - Price


                var taxes = new List<ReportColumnDto>();
                taxes.Add(new ReportColumnDto { FirstColumn = "Net", SecondColumn = "EUR" + string.Format("{0:0.00}", totalPrice) });
                foreach (var item in taxRates)
                    taxes.Add(new ReportColumnDto { FirstColumn = item.FirstColumn, SecondColumn = item.SecondColumn });
                taxes.Add(new ReportColumnDto { FirstColumn = "Revenue gross", SecondColumn = "EUR" + revenue });
                taxes.Add(new ReportColumnDto { FirstColumn = "Net total", SecondColumn = "EUR" + string.Format("{0:0.00}", totalPrice) });
                taxes.Add(new ReportColumnDto { FirstColumn = "Tax total", SecondColumn = "EUR" + string.Format("{0:0.00}", totalTaxRate) });
                taxes.Add(new ReportColumnDto { FirstColumn = "Revenue", SecondColumn = "EUR" + revenue });
                reportDatasDto.Taxes = taxes;



                var revenueByEmployees = new List<ReportColumnDto>();
                revenueByEmployees.Add(new ReportColumnDto { FirstColumn = "Solmaz LTD.", SecondColumn = data.Count().ToString(), ThirdColumn = "EUR" + revenue });
                revenueByEmployees.Add(new ReportColumnDto { FirstColumn = "Total", SecondColumn = data.Count().ToString(), ThirdColumn = "EUR" + revenue });
                reportDatasDto.RevenueByEmployees = revenueByEmployees;



                reportDatasDto.RevenueByCategories = data.GroupBy(x => x.Product.Category.Name).Select(g => new ReportColumnDto
                {
                    FirstColumn = g.Key,
                    SecondColumn = g.Count().ToString(),
                    ThirdColumn = "EUR" + string.Format("{0:0.00}", g.Sum(a => a.Count * a.Price * (1 + (decimal)a.Product.TaxRate / 100)))
                    //ThirdColumn = "EUR" + g.Sum(a => a.Count * a.Price * (1 + Decimal.Parse((a.Product.TaxRate / 100).ToString()))).ToString()
                }).ToList(); // CategoryName - Count - Price




                reportDatasDto.PaymentMethods = data.GroupBy(x => x.Order.PayType).Select(g => new ReportColumnDto
                {
                    FirstColumn = g.Key,
                    SecondColumn = g.Count().ToString(),
                    ThirdColumn = "EUR" + string.Format("{0:0.00}", g.Sum(a => a.Count * a.Price * (1 + (decimal)a.Product.TaxRate / 100)))
                }).ToList(); // CurrencyUnitName - Count - Price



                result.Data = reportDatasDto;
                result.Success = true;
                result.Message = ResultMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }
    }
}