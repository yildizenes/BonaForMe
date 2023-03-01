using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace BonaForMe.ServiceCore.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly BonaForMeDBContext _context;
        public AdminService(BonaForMeDBContext context)
        {
            _context = context;
        }

        public Result<DashboardDto> GetDashboardData()
        {
            Result<DashboardDto> result = new Result<DashboardDto>();
            var model = new DashboardDto();
            var thisMonth = DateTime.Now.Month;
            try
            {
                model.PendingApprovalUserCount = _context.Users.Count(x => !x.IsApproved && x.IsActive && !x.IsDeleted);
                model.NewOrdersCount = _context.Orders.Count(x => x.OrderStatusId == 1 && x.IsActive && !x.IsDeleted);
                model.OrdersCountOfThisMonth = _context.Orders.Count(x => x.DateCreated.Month == thisMonth && x.OrderStatusId > 4 && x.IsActive && !x.IsDeleted);
                model.TotalOrderCount = _context.Orders.Count(x => x.IsActive && !x.IsDeleted);

                result.Data = model;
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