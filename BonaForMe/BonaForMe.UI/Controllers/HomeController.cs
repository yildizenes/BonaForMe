using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.ApplicationSettingService;
using Microsoft.AspNetCore.Mvc;

namespace BonaForMe.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationSettingService _applicationSettingService;
        public HomeController(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
        }
        public IActionResult Index()
        {
            var settings = _applicationSettingService.GetApplicationSetting();
            var model = new ApplicationSettingDto
            {
                InfoMail = settings.Data?.InfoMail,
                Telephone = settings.Data?.Telephone,
                Facebook = settings.Data?.Facebook,
                LinkedIn = settings.Data?.LinkedIn,
                Twitter = settings.Data?.Twitter,
                Instagram = settings.Data?.Instagram,
                PlayStoreAddress = settings.Data?.PlayStoreAddress,
                AppleStoreAddress = settings.Data?.AppleStoreAddress,
                HuaweiStoreAddress = settings.Data?.HuaweiStoreAddress
            };

            return View(model);
        }

        public IActionResult Shop()
        {
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