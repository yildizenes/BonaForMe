﻿using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.PaymentInfoService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace BonaForMe.UI.Controllers
{
    [Authorize]
    public class PaymentInfoController : Controller
    {
        private readonly IPaymentInfoService _paymentInfoService;
        public PaymentInfoController(IPaymentInfoService paymentInfoService)
        {
            _paymentInfoService = paymentInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(PaymentInfoDto paymentInfoDto)
        {
            try
            {
                var result = _paymentInfoService.AddPaymentInfo(paymentInfoDto);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Update(PaymentInfoDto paymentInfoDto)
        {
            try
            {
                var result = _paymentInfoService.UpdatePaymentInfo(paymentInfoDto);
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
                var result = _paymentInfoService.DeletePaymentInfo(id);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult GetPaymentInfoById(Guid id)
        {
            try
            {
                var result = _paymentInfoService.GetPaymentInfoById(id);
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
        public JsonResult GetAllPaymentInfo()
        {
            try
            {
                var result = _paymentInfoService.GetAllPaymentInfo();
                return new JsonResult(result.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult LoadPaymentInfoData()
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
                return _paymentInfoService.FillDataTable(dataTable);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}