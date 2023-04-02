using BonaForMe.DomainCommonCore.Helper;
using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BonaForMe.UI.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(UserDto userDto)
        {
            try
            {
                if (userDto == null)
                    return Json(new { success = false });

                if (!userDto.UserMail.Contains("@") && !userDto.UserMail.Contains(".com"))
                    return Json(new { success = false, message = "Please enter a valid email address" });

                if (userDto.Id == Guid.Empty)
                {
                    var mailCheck = _userService.GetUserByEmail(userDto.UserMail);
                    if (mailCheck.Data != null)
                        return Json(new { success = false, message = "The entered e-mail address is registered in the system with another user." });
                }

                if (userDto.Id == Guid.Empty)
                    userDto.UserPassword = PasswordHelper.PasswordEncoder(userDto.UserPassword);
                var result = _userService.AddUser(userDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Update(UserDto userDto)
        {
            try
            {
                var result = _userService.UpdateUser(userDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var result = _userService.DeleteUser(id);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult GetUserById(Guid id)
        {
            try
            {
                var result = _userService.GetUserById(id);
                if (result != null)
                {
                    return new JsonResult(result.Data);
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetAllUser()
        {
            try
            {
                var result = _userService.GetAllUser();
                return new JsonResult(result.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult LoadUserData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                //Paging Size (10,20,50,100)
                int pageSize = Convert.ToInt32(length) != -1 ? Convert.ToInt32(length) : 100;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                DataTableDto dataTable = new DataTableDto()
                {
                    Draw = draw,
                    PageSize = pageSize,
                    Skip = skip,
                    SearchValue = searchValue,
                    SortColumnDirection = sortColumnDirection,
                    SortColumn = sortColumn
                };
                return _userService.FillDataTable(dataTable);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetAllCustomer()
        {
            try
            {
                var result = _userService.GetAllCustomer();
                return new JsonResult(result.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}