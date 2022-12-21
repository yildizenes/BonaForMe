using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.OrderService;
using BonaForMe.ServiceCore.UserService;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace BonaForMe.ServiceCore.HomeService
{
    public class HomeService : IHomeService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;
        private IUserService _userService;
        private IOrderService _orderService;

        public HomeService(BonaForMeDBContext context, IUserService userService, IOrderService orderService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _orderService = orderService;
        }

        public Result<DashboardDto> GetDashboardData()
        {
            Result<DashboardDto> result = new Result<DashboardDto>();
            var model = new DashboardDto();
            var thisMonth = DateTime.Now.Month;
            try
            {
                model.UserCount = _context.Users.Count(x => x.IsActive && !x.IsDeleted);
                model.NewOrdersCount = _context.Orders.Count(x => x.OrderStatusId == 1 && x.IsActive && !x.IsDeleted);
                model.OrdersCountOfThisMonth = _context.Orders.Count(x => x.DateCreated.Value.Month == thisMonth && x.OrderStatusId > 4 && x.IsActive && !x.IsDeleted);
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