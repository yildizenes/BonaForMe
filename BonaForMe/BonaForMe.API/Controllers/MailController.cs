using BonaForMe.DomainCore.DTO;
using BonaForMe.ServiceCore.MailSenderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BonaForMe.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    public class MailController : Controller
    {
        private readonly IMailSenderService _mailSenderService;
        public MailController(IMailSenderService mailSenderService)
        {
            _mailSenderService = mailSenderService;
        }

        [HttpPost]
        public IActionResult AddPaymentInfo(MailSenderDto mailSenderDto)
        {
            try
            {
                var result = _mailSenderService.SendMail(mailSenderDto.ToMailAddress, (MailTypes)mailSenderDto.MailTypes, mailSenderDto.Notification);
                return Json(new { success = result.Success, data = "", message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }
    }
}