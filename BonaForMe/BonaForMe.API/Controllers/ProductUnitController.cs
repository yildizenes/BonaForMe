using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.ProductUnitService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class ProductUnitController : Controller
    {
        private readonly IProductUnitService _productUnitService;
        public ProductUnitController(IProductUnitService productUnitService)
        {
            _productUnitService = productUnitService;
        }

        [HttpGet]
        public IActionResult GetAllProductUnit()
        {
            try
            {
                var result = _productUnitService.GetAllProductUnit();
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetProductUnitById(int id)
        {
            try
            {
                if (id == 0)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _productUnitService.GetProductUnitById(id);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddProductUnit(ProductUnitDto productUnitDto)
        {
            try
            {
                var result = _productUnitService.AddProductUnit(productUnitDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateProductUnit(ProductUnitDto productUnitDto)
        {
            try
            {
                var result = _productUnitService.UpdateProductUnit(productUnitDto);
                return Json(new { success = result.Success, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteProductUnit(int id)
        {
            try
            {
                if (id == 0)
                    return Json(new { Success = false, Data = "", Message = "Request parameter is not found." });

                var result = _productUnitService.DeleteProductUnit(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}