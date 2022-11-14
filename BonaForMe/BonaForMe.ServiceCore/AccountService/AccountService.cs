using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.DomainCore.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Microsoft.AspNetCore.Mvc;
using BonaForMe.ServiceCore.UserService;

namespace BonaForMe.ServiceCore.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly BonaForMeDBContext _context;
        IMapper _mapper;
        private IUserService _userService;

        public AccountService(BonaForMeDBContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public Result<UserDto> Login(AccountDto accountDto)
        {
            Result<UserDto> result = new Result<UserDto>();
            try
            {
                var model = _context.Users.Where(x => x.UserMail == accountDto.UserMail && x.UserPassword == accountDto.UserPassword && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (model != null)
                {
                    result.Data = _mapper.Map<UserDto>(model);
                    result.Success = true;
                    result.Message = ResultMessages.Success;
                }
                else
                {
                    result.Success = false;
                }
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