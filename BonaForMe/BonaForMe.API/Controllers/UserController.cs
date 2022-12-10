using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.AccountService;
using BonaForMe.ServiceCore.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        public UserController(IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetUser(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _userService.GetUserById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditUser(UserDto userDto)
        {
            try
            {
                if (userDto == null)
                    return Json(new { success = false });

                var result = _userService.UpdateUser(userDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = "" });
            }
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            try
            {
                var userList = _userService.GetAllUser();
                return Json(new { data = userList, success = true, message = "Process is Success" });
            }
            catch (Exception ex)
            {
                return Json(new { data = "", success = false, message = ex.Message });
            }
        }
    }
}