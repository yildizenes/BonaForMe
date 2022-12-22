using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BonaForMe.ServiceCore.UserService
{
    public interface IUserService
    {
        Result<UserDto> AddUser(UserDto userDto);
        Result DeleteUser(Guid id);
        Result<UserDto> UpdateUser(UserDto userDto, bool isResetPassword = false);
        Result<UserDto> GetUserById(Guid id);
        Result<List<UserDto>> GetAllUser();
        JsonResult FillDataTable(DataTableDto dataTable);
        Result<UserDto> GetUserByEmail(string userMail);
        Result ChangeApproveStatus(Guid userId, bool isApprove);
    }
}
