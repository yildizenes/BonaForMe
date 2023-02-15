using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;

namespace BonaForMe.ServiceCore.ReportService
{
    public interface IReportService
    {
        Result<ReportDto> GetSalesValue(ReportDateDto reportDateDto);
        Result<byte[]> CreateReport(ReportDateDto reportDateDto);
    }
}
