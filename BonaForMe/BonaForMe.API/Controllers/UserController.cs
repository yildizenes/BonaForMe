using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BonaForMe.DomainCommonCore.Constants;
using BonaForMe.DomainCommonCore.Enums;
using BonaForMe.DomainCommonCore.Helper;
using BonaForMe.DomainCommonCore.Result;
using BonaForMe.DomainCore.DBModel;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.BonaForMeInfoService;
using BonaForMe.ServiceCore.UserService;
using BonaForMe.ServicesCore.LogService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        private readonly IBonaForMeInfoService _bonaForMeInfoService;
        public UserController(IUserService userService, ILogService logService, IBonaForMeInfoService bonaForMeInfoService)
        {
            _userService = userService;
            _logService = logService;
            _bonaForMeInfoService = bonaForMeInfoService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<string> Index()
        {
            return new string[] { "Bona For Me", "Api" };
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(Guid id)
        {
            Result<User> result = new Result<User>();

            try
            {
                if (id == Guid.Empty)
                {
                    result.Data = null;
                    result.Message = "Request Paramaters are not found";
                    result.Success = false;

                    return Json(data: result);
                }

                var userInfo = _userService.GetUserById(id);
                if (userInfo != null)
                {
                    result.Data = userInfo;
                    result.Message = "Process is success";
                    result.Success = true;
                    return Json(data: result);
                }
                else
                {
                    result.Data = null;
                    result.Message = "User is Not Found";
                    result.Success = false;

                    return Json(data: result);
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Message = ex.Message;
                result.Success = false;

                return Json(data: result);
            }
        }

        [HttpPost]
        public JsonResult EditUser(UserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    return Json(new { success = false });
                }

                if (userDTO.b64File != null)
                {

                    byte[] b;

                    b = Convert.FromBase64String(userDTO.b64File);

                    userDTO.UserPicture = b;
                }

                var result = _userService.UpdateUser(userDTO);
                if (result.Success != false)
                {
                    _logService.SaveLog(userDTO.Id, "USER.ID", "User Edit Process is Success", AppEnums.LogTypes.Success, "UserController.EditUser", "", userDTO.Id);

                    return Json(new { success = true, message = "Process is Success", data = result.Data });
                }
                else
                {
                    _logService.SaveLog(userDTO.Id, "USER.ID", "User Edit Process is Failed", AppEnums.LogTypes.UnSuccess, "UserController.EditUser", "", userDTO.Id, result.Message);
                    return Json(new { success = false, message = "Process is Failed", data = "" });
                }
            }
            catch (Exception ex)
            {
                _logService.SaveLog(userDTO.Id, "USER.ID", "User Edit Process is Error", AppEnums.LogTypes.Error, "UserController.EditUser", "", userDTO.Id, ex.Message, true);

                return Json(new { success = false, message = ex.Message, data = "" });
            }
        }

        [HttpGet()]
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserDTO model)
        {
            try
            {
                var hashPassword = PasswordHelper.PasswordEncoder(model.UserPassword);

                var userInfo = _userService.Login(model.UserMail, hashPassword);

                if (userInfo.Success == true)
                {

                    var bonaForMeNoInfo = _bonaForMeInfoService.GetBonaForMeInfoByUserId(userInfo.Data.Id);

                    if(bonaForMeNoInfo == null) 
                    {
                        return new JsonResult(new { result = false, token = "", user = "", message="Bona For Me bilgisi bulunamadı Lütfen daha sonra tekrar deneyiniz!" });
                    }

                    userInfo.Data.BonaForMeNo = bonaForMeNoInfo.BonaForMeNo;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.GivenName, userInfo.Data.UserName+" "+userInfo.Data.UserLastName),
                        new Claim(ClaimTypes.PrimarySid, userInfo.Data.Id.ToString()),
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

                    return new JsonResult(new { result = true, token = new JwtSecurityTokenHandler().WriteToken(token), user = userInfo.Data });
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
        public JsonResult Register(UserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    return Json(new { success = false });
                }


                if (!userDTO.UserMail.Contains("@") && !userDTO.UserMail.Contains(".com"))
                {
                    return Json(new { success = false, message = "Please enter a valid email address" });
                }

                var mailCheck = _userService.GetUserByEmail(userDTO.UserMail);
                if (mailCheck != null)
                {
                    return Json(new { success = false, message = "There is Entered value already on the system" });
                }

                var pId = userDTO.BonaForMeNo;
                var bonaForMeInfo = _bonaForMeInfoService.GetBonaForMeInfoByBonaForMeNo(pId);
                if (bonaForMeInfo == null)
                {
                    return Json(new { success = false, message = "Bona For Me bilgisi bulunamadı. Bona For Me yetkilisi ile iletişime geçiniz!", data = "" });
                }
         
                userDTO.UserProfileShowStatus = true;
                userDTO.UserType = (int)AppEnums.UserTypes.User;

                var result = _userService.AddUser(userDTO);
                if (result.Success != false)
                {
                     bonaForMeInfo.UserId = result.Data.Id;
                    _bonaForMeInfoService.UpdateBonaForMeInfo(bonaForMeInfo);
                    _logService.SaveLog(result.Data.Id, "USER.ID", "User Register Process is Success", AppEnums.LogTypes.Success, "UserController.Register", "", result.Data.Id);

                    return Json(new { success = true, message = "Process is Success", data = result.Data });
                }
                else
                {
                    _logService.SaveLog(Guid.Empty, "USER.ID", "User Register Process is Failed", AppEnums.LogTypes.UnSuccess, "UserController.Register", "", Guid.Empty, result.Message);
                    return Json(new { success = false, message = "Process is Failed", data = "" });
                }
            }
            catch (Exception ex)
            {
                _logService.SaveLog(Guid.Empty, "USER.ID", "User Register Process is Error", AppEnums.LogTypes.Error, "UserController.Register", "",Guid.Empty, ex.Message, true);

                return Json(new { success = false, message = ex.Message, data = "" });
            }
        }

        [AllowAnonymous]
        [HttpPost("{mail}")]
        public JsonResult ForgetPassword(string mail)
        {
            try
            {

                var userInfo = _userService.GetUserByEmail(mail);
                if (userInfo == null)
                {
                    return Json(new { success = false, message = "This email address is not found on the system! Please check your email address." });
                }

                Random rnd = new Random();
                string randomPassword = "";
                for (int i = 0; i < 8; i++)
                {
                    randomPassword += rnd.Next(0, 9).ToString();
                }

                var model = new UserDTO
                {
                    Id = userInfo.Id,
                    UserMail = userInfo.UserMail,
                    UserPassword = randomPassword,
                    UserName = userInfo.UserName,
                    UserLastName = userInfo.UserLastName,
                    isForgetPasswordOperation = true,
                    UserBio = userInfo.UserBio,
                    UserPhone = userInfo.UserPhone,
                    UserBloodType = userInfo.UserBloodType
                };

                var result = _userService.AddUser(model);

                EmailHelper.SendForgetPasswordMail(mail, randomPassword);

                if (result.Success != false)
                {
                    _logService.SaveLog(result.Data.Id, "USER.ID", "User Forget Password Process is Success", AppEnums.LogTypes.Success, "UserController.ForgetPassword", "", result.Data.Id);

                    return Json(new { success = true, message = "Process is Success", data = result.Data });
                }
                else
                {
                    _logService.SaveLog(Guid.Empty, "USER.ID", "User Forget Password Process is Failed", AppEnums.LogTypes.UnSuccess, "UserController.ForgetPassword", "", Guid.Empty, result.Message);
                    return Json(new { success = false, message = "Process is Failed", data = "" });
                }
            }
            catch (Exception ex)
            {
                _logService.SaveLog(Guid.Empty, "USER.ID", "User Forget Password Process is Error", AppEnums.LogTypes.Error, "UserController.ForgetPassword", "", Guid.Empty, ex.Message, true);

                return Json(new { success = false, message = ex.Message, data = "" });
            }
        }

    }
}
