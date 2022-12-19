using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;

namespace BonaForMe.ServiceCore.HomeService
{
    public interface IHomeService
    {
        Result<DashboardDto> GetDashboardData();
    }
}
