using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.ReportService
{
    public interface IReportService
    {
        Result<ReportDto> GetSalesValue(ReportDateDto reportDateDto);
        Result<byte[]> CreateReport(ReportDateDto reportDateDto);
        Result<ReportDatasDto> ReportValue(ReportDateDto reportDateDto);
    }
}