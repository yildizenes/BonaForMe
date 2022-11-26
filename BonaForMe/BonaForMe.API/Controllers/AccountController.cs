using BonaForMe.DomainCommonCore.Constants;
using BonaForMe.DomainCommonCore.Helper;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.AccountService;
using BonaForMe.ServiceCore.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        public AccountController(IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AccountDto accountDto)
        {
            try
            {
                accountDto.UserPassword = PasswordHelper.PasswordEncoder(accountDto.UserPassword);
                var result = _accountService.Login(accountDto);
                if (result.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.PrimarySid, result.Data.Id.ToString()),
                        new Claim(ClaimTypes.Name, result.Data.FullName),
                        new Claim(ClaimTypes.Email, result.Data.UserMail),
                        new Claim("IsAdmin", result.Data.IsAdmin.ToString(), ClaimValueTypes.Boolean),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(JwtSettings.Issuer,
                      JwtSettings.Issuer,
                      claims,
                      expires: DateTime.Now.AddDays(20),
                      signingCredentials: creds);

                    //model.CurrentToken = Guid.Parse(new JwtSecurityTokenHandler().WriteToken(token));
                    //_userService.AddUser(userInfo.Data);

                    return new JsonResult(new { result = true, token = new JwtSecurityTokenHandler().WriteToken(token), user = result.Data });
                }
                else
                {
                    return new JsonResult(new { result = false, token = "", user = "" });
                }
            }
            catch (Exception e)
            {
                return new JsonResult(new { result = false, token = "", user = "", message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SignIn(UserDto userDto)
        {
            try
            {
                if (userDto == null)
                    return Json(new { success = false });

                if (!userDto.UserMail.Contains("@") && !userDto.UserMail.Contains(".com"))
                    return Json(new { success = false, message = "Please enter a valid email address" });

                var mailCheck = _userService.GetUserByEmail(userDto.UserMail);
                if (mailCheck != null)
                    return Json(new { success = false, message = "There is Entered value already on the system" });

                var result = _userService.AddUser(userDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = "" });
            }
        }

        [AllowAnonymous]
        [HttpPost("{mail}")]
        public JsonResult ForgetPassword(string mail)
        {
            try
            {
                var userData = _userService.GetUserByEmail(mail);
                if (userData == null)
                    return Json(new { success = false, message = "This email address is not found on the system! Please check your email address." });

                string userNewPassword = PasswordHelper.GeneratePassword();
                userData.Data.UserPassword = PasswordHelper.PasswordEncoder(userNewPassword);
                var result = _userService.UpdateUser(userData.Data);

                EmailHelper.SendForgetPasswordMail(mail, userNewPassword);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = "" });
            }
        }
    }
}