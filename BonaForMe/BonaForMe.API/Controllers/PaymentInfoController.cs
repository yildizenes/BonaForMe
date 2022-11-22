using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.PaymentInfoService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class PaymentInfoController : Controller
    {
        private readonly IPaymentInfoService _paymentInfoService;
        public PaymentInfoController(IPaymentInfoService paymentInfoService)
        {
            _paymentInfoService = paymentInfoService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllPaymentInfo()
        {
            try
            {
                var result = _paymentInfoService.GetAllPaymentInfo();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetPaymentInfoById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _paymentInfoService.GetPaymentInfoById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddPaymentInfo(PaymentInfoDto paymentInfoDto)
        {
            try
            {
                var result = _paymentInfoService.AddPaymentInfo(paymentInfoDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdatePaymentInfo(PaymentInfoDto paymentInfoDto)
        {
            try
            {
                var result = _paymentInfoService.UpdatePaymentInfo(paymentInfoDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost("{id}")]
        public IActionResult DeletePaymentInfo(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _paymentInfoService.DeletePaymentInfo(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}