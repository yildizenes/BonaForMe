using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.CurrencyUnitService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class CurrencyUnitController : Controller
    {
        private readonly ICurrencyUnitService _currencyUnitService;
        public CurrencyUnitController(ICurrencyUnitService currencyUnitService)
        {
            _currencyUnitService = currencyUnitService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllCurrencyUnit()
        {
            try
            {
                var result = _currencyUnitService.GetAllCurrencyUnit();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCurrencyUnitById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _currencyUnitService.GetCurrencyUnitById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddCurrencyUnit(CurrencyUnitDto currencyUnitDto)
        {
            try
            {
                var result = _currencyUnitService.AddCurrencyUnit(currencyUnitDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateCurrencyUnit(CurrencyUnitDto currencyUnitDto)
        {
            try
            {
                var result = _currencyUnitService.UpdateCurrencyUnit(currencyUnitDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCurrencyUnit(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _currencyUnitService.DeleteCurrencyUnit(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}