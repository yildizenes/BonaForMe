using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.OrderLogService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BonaForMe.UI.Controllers
{
    [Authorize]
    public class OrderLogController : Controller
    {
        private readonly IOrderLogService _orderLogService;
        public OrderLogController(IOrderLogService orderLogService)
        {
            _orderLogService = orderLogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(OrderLogDto orderLogDto)
        {
            try
            {
                var result = _orderLogService.AddOrderLog(orderLogDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Update(OrderLogDto orderLogDto)
        {
            try
            {
                var result = _orderLogService.UpdateOrderLog(orderLogDto);
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
                var result = _orderLogService.DeleteOrderLog(id);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult GetOrderLogById(Guid id)
        {
            try
            {
                var result = _orderLogService.GetOrderLogById(id);
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
        public JsonResult GetAllOrderLog()
        {
            try
            {
                var result = _orderLogService.GetAllOrderLog();
                return new JsonResult(result.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult LoadOrderLogData()
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
                return _orderLogService.FillDataTable(dataTable);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}