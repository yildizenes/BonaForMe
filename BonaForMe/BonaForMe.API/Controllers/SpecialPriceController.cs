using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.SpecialPriceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class SpecialPriceController : Controller
    {
        private readonly ISpecialPriceService _specialPriceService;
        public SpecialPriceController(ISpecialPriceService specialPriceService)
        {
            _specialPriceService = specialPriceService;
        }

        [HttpGet]
        public IActionResult GetAllSpecialPrice()
        {
            try
            {
                var result = _specialPriceService.GetAllSpecialPrice();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetSpecialPriceById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _specialPriceService.GetSpecialPriceById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddSpecialPrice(SpecialPriceDto specialPriceDto)
        {
            try
            {
                var result = _specialPriceService.AddSpecialPrice(specialPriceDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateSpecialPrice(SpecialPriceDto specialPriceDto)
        {
            try
            {
                var result = _specialPriceService.UpdateSpecialPrice(specialPriceDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteSpecialPrice(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _specialPriceService.DeleteSpecialPrice(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}