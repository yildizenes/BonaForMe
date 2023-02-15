using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using IronXL;

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
                    .Where(x => x.IsActive && !x.IsDeleted).ToList();

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
                WorkBook workbook = WorkBook.Create(ExcelFileFormat.XLSX);
                var sheet = workbook.CreateWorkSheet("Result Sheet");

                // Set titles manually
                sheet["A1"].Value = "Account";
                sheet["B1"].Value = "Date";
                sheet["C1"].Value = "Time";
                sheet["D1"].Value = "Type";
                sheet["E1"].Value = "Order Code";
                sheet["F1"].Value = "Payment Method";
                sheet["G1"].Value = "Quantity";
                sheet["H1"].Value = "Description";
                sheet["I1"].Value = "Currency";
                sheet["J1"].Value = "Price(Gross)";
                sheet["K1"].Value = "Price(Net)";
                sheet["L1"].Value = "Tax";
                sheet["M1"].Value = "Tax Rate";

                // Get Values
                var values = _context.OrderLogs
                    .Include(x => x.Order)
                    .Include(x => x.Product).ThenInclude(x=> x.CurrencyUnit)
                    .Where(x => x.DateCreated >= reportDateDto.StartDate && x.DateCreated <= reportDateDto.EndDate 
                                && x.IsActive && !x.IsDeleted).ToList();

                int index = 2;
                foreach (var item in values)    // Set cell values
                {
                    var taxPrice = (item.Price * item.Count * item.Product.TaxRate) / 100;

                    sheet["A" + index].Value = "solmazpackaging@gmail.com";
                    sheet["B" + index].Value = item.DateCreated.Value.ToShortDateString();
                    sheet["C" + index].StringValue = item.DateCreated.Value.ToShortTimeString();
                    sheet["D" + index].Value = "Sales";
                    sheet["E" + index].Value = item.Order.OrderCode;
                    sheet["F" + index].Value = item.Order.PayType;
                    sheet["G" + index].Value = item.Count;
                    sheet["H" + index].Value = item.Product.Name;
                    sheet["I" + index].Value = item.Product.CurrencyUnit.Name;
                    sheet["J" + index].Value = item.Price + taxPrice;
                    sheet["K" + index].Value = item.Price;
                    sheet["L" + index].Value = taxPrice;
                    sheet["M" + index].Value = item.Product.TaxRate;

                    index++;
                }

                sheet.Rows[0].Style.Font.Bold = true;
                foreach (var item in sheet.Columns)
                    item.Width = 4000;

                result.Data = workbook.ToByteArray();
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