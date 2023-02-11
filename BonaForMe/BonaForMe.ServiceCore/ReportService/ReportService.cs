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
        public Result CreateExcel(ReportDateDto reportDateDto)
        {
            Result result = new Result();
            try
            {

                WorkBook workbook = WorkBook.Create(ExcelFileFormat.XLSX);
                var sheet = workbook.CreateWorkSheet("Result Sheet"); 

                // Set Titles Manually
                sheet["A1"].Value = "Object Oriented Programming";
                sheet["B1"].Value = "Data Structure";
                sheet["C1"].Value = "Database Management System";

                for (int i = 2; i <= 4; i++)    // Set Cell Values
                {
                    sheet["A" + i].Value = "Test - " + i;
                    sheet["B" + i].Value = "Test - " + i;
                    sheet["C" + i].Value = "Test - " + i;
                }

                workbook.SaveAs("ResultSheet.xlsx");    // Save Workbook

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