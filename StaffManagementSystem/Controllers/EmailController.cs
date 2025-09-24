using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.Interfaces;

namespace StaffManagementSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : BaseController
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailDto request)
        {
            await _emailService.SendEmailAsync(request);
            return Success("Email has been sent successfully.");
        }
    }
}
