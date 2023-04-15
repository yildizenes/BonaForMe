using BonaForMe.DomainCommonCore.CustomClass;
using BonaForMe.DomainCommonCore.Helper;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.AccountService;
using BonaForMe.ServiceCore.MailSenderService;
using BonaForMe.ServiceCore.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BonaForMe.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly IMailSenderService _mailSenderService;
        public AccountController(IUserService userService, IAccountService accountService,
            IMailSenderService mailSenderService)
        {
            _userService = userService;
            _accountService = accountService;
            _mailSenderService = mailSenderService;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            var successInfo = TempData["Success"];
            if (successInfo != null)
            {
                ViewBag.Success = successInfo;
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //return RedirectToAction(nameof(UserAccountController.Login), "Account");
            return RedirectToAction("Login"); // Değişecek
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccountDto accountDto)
        {
            var returnTo = "/Account/Login";

            if (!accountDto.UserMail.Contains("@") && !accountDto.UserMail.Contains(".com"))
            {
                TempData["Error"] = "Please enter a valid email address!";
                return RedirectToAction("Login", "Account");
            }
            try
            {
                accountDto.UserPassword = PasswordHelper.PasswordEncoder(accountDto.UserPassword);
                var result = _accountService.Login(accountDto);
                if (result.Success)
                {
                    if (!result.Data.IsApproved)
                    {
                        TempData["Error"] = "Your account is awaiting approval.";
                        return RedirectToAction("Login", "Account");
                    }
                    if (!result.Data.IsAdmin)
                    {
                        TempData["Error"] = "Only admin users can login.";
                        return RedirectToAction("Login", "Account");
                    }
                    await CreateClaims(result.Data);
                    returnTo = "/Admin/Dashboard";
                }
                else
                {
                    TempData["Error"] = "Email address or password is incorrect.";
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "There was an error in the login process!";
                return RedirectToAction("Login", "Account");
            }

            return Redirect(returnTo);
        }

        [AllowAnonymous]
        public IActionResult SignIn()
        {
            ViewBag.Error = TempData["Error"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SignIn(UserDto userDto)
        {
            var returnTo = "/Account/Login";
            try
            {
                if (!userDto.UserMail.Contains("@") && !userDto.UserMail.Contains(".com"))
                {
                    TempData["Error"] = "Please enter a valid email address!";
                    return RedirectToAction("SignIn", "Account");
                }

                var mailCheck = _userService.GetUserByEmail(userDto.UserMail);
                if (mailCheck.Data != null)
                {
                    TempData["Error"] = "The entered e-mail address is registered in the system with another user!";
                    return RedirectToAction("SignIn", "Account");
                }
                userDto.UserPassword = PasswordHelper.PasswordEncoder(userDto.UserPassword);
                var result = _userService.AddUser(userDto).Data;
                if (result != null)
                {
                    returnTo = "/ProfilePage?pId=" + result.Id;
                }
                else
                {
                    TempData["Error"] = "Error creating new user!";
                    return RedirectToAction("SignIn", "Account");
                }
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error creating new user!" + " " + e.Message;
                return RedirectToAction("SignIn", "Account");
            }

            return Redirect(returnTo);
        }

        [AllowAnonymous]
        [Route("~/ProfilePage")]
        public IActionResult ProfilePage(Guid userId)
        {
            var userData = _userService.GetUserById(userId);
            return View(userData);
        }

        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            var errorInfo = TempData["Error"];
            if (errorInfo != null)
            {
                ViewBag.Error = errorInfo;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ForgetPassword(string userMail)
        {
            try
            {
                var userData = _userService.GetUserByEmail(userMail);
                if (userData.Data == null)
                {
                    TempData["Error"] = "This email address is not found on the system! Please check your email address.";
                    return RedirectToAction("ForgetPassword", "Account");
                }

                string userNewPassword = PasswordHelper.GeneratePassword();
                userData.Data.UserPassword = PasswordHelper.PasswordEncoder(userNewPassword);
                _userService.UpdateUser(userData.Data, true);
                _mailSenderService.SendMail(userMail, MailTypes.ForgetPassword, userNewPassword);

                TempData["Success"] = "Your new password has been sent to your e-mail address. Note: Mail may go to spam (junk) box!";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                TempData["Error"] = "There was an error in password reset. Please try again later!";
                return RedirectToAction("ForgetPassword", "Account");
            }
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = CurrentUser.Get(HttpContext.User);
                resetPasswordDto.UserId = Guid.Parse(user.UserId);
                resetPasswordDto.OldPassword = PasswordHelper.PasswordEncoder(resetPasswordDto.OldPassword);
                resetPasswordDto.NewPassword = PasswordHelper.PasswordEncoder(resetPasswordDto.NewPassword);
                resetPasswordDto.VerificationNewPassword = PasswordHelper.PasswordEncoder(resetPasswordDto.VerificationNewPassword);

                var result = _accountService.ResetPassword(resetPasswordDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task CreateClaims(UserDto userDto)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.PrimarySid, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.FullName),
                new Claim(ClaimTypes.Email, userDto.UserMail),
                new Claim("IsAdmin", userDto.IsAdmin.ToString(), ClaimValueTypes.Boolean),
                new Claim("IsCourier", userDto.IsCourier.ToString(), ClaimValueTypes.Boolean),
            };
            await SetClaims(claims);
        }

        private async Task SetClaims(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [HttpPost]
        public JsonResult ChangeApproveStatus(Guid userId, bool isApproved)
        {
            try
            {
                var result = _userService.ChangeApproveStatus(userId, isApproved);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
