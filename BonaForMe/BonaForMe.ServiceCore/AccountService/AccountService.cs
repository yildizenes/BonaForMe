using AutoMapper;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.UserService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

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

        public Result<ResetPasswordDto> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            Result<ResetPasswordDto> result = new Result<ResetPasswordDto>();
            try
            {
                var model = _context.Users.FirstOrDefault(x => x.Id == resetPasswordDto.UserId && x.IsActive && !x.IsDeleted);

                if (model != null)
                {
                    if (model.UserPassword != resetPasswordDto.OldPassword)
                    {
                        result.Success = false;
                        result.Message = "Old password wrong!";
                        return result;
                    }

                    model.UserPassword = resetPasswordDto.NewPassword;
                    _context.Entry(model).State = EntityState.Detached;
                    _context.Update(model);
                    _context.SaveChanges();

                    result.Data = resetPasswordDto;
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