using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;

namespace BonaForMe.ServiceCore.AdminService
{
    public interface IAdminService
    {
        Result<DashboardDto> GetDashboardData();
    }
}
