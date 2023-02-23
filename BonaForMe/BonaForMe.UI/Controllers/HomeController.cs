using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.ApplicationSettingService;
using BonaForMe.ServiceCore.CategoryService;
using BonaForMe.ServiceCore.ProductService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BonaForMe.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        public HomeController(IApplicationSettingService applicationSettingService, ICategoryService categoryService, IProductService productService)
        {
            _applicationSettingService = applicationSettingService;
            _categoryService = categoryService;
            _productService = productService;
        }
        public IActionResult Index()
        {
            var settings = _applicationSettingService.GetApplicationSetting();
            var model = new ApplicationSettingDto
            {
                Telephone = settings.Data?.Telephone,
                OpenCloseTime = settings.Data?.OpenCloseTime,
            };

            var allProducts = _productService.GetAllProduct()?.Data;
            ViewBag.Products = allProducts.OrderByDescending(p => p.DateCreated).Take(12);
            ViewBag.Categories = _categoryService.GetAllCategory()?.Data;
            
            return View(model);
        }

        [HttpGet("Shop/")]
        [HttpGet("Shop/categories/{categoryId}")]
        [HttpGet("Shop/categories/{categoryId}/{searchValue}")]
        public IActionResult Shop(Guid? categoryId, string searchValue)
        {
            ViewBag.Categories = _categoryService.GetAllCategory()?.Data;
            List<ProductDto> products = new List<ProductDto>();
            if (categoryId == null || categoryId == Guid.Empty)
                products = _productService.GetAllProduct()?.Data;
            else
                products = _productService.GetAllProductByCategoryId((Guid)categoryId)?.Data;

            if (searchValue != null)
                products = products.Where(m => m.Name.ToLower().Contains(searchValue.ToLower()) || m.Description.ToLower().Contains(searchValue.ToLower())).ToList();

            ViewBag.Products = products;

            return View();
        }

        public IActionResult About()
        {
            var settings = _applicationSettingService.GetApplicationSetting();
            var model = new ApplicationSettingDto
            {
                AboutUs = settings.Data?.AboutUs
            };

            return View(model);
        }

        public IActionResult Presentation()
        {
            var settings = _applicationSettingService.GetApplicationSetting();
            var model = new ApplicationSettingDto
            {
                Vision = settings.Data?.Mission,
                Mission = settings.Data?.Vision,
            };

            return View(model);
        }

        public IActionResult Contact()
        {
            var settings = _applicationSettingService.GetApplicationSetting();
            var model = new ApplicationSettingDto
            {
                Address = settings.Data?.Address,
                InfoMail = settings.Data?.InfoMail,
                Telephone = settings.Data?.Telephone,
                GoogleMaps = settings.Data?.GoogleMaps,
                OpenCloseTime = settings.Data?.OpenCloseTime,
            };

            return View(model);
        }
    }
}