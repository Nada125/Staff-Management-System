using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.Interfaces;

namespace StaffManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            return result.Success
                ? Success(result.Data, result.Message)
                : Error(result.Message);
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto)
        {
            var result = await _authService.VerifyCodeAsync(dto.Email, dto.Code, dto.Purpose);

            return result.Success
                ? Success(result.Message)
                : Error(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);

            return result.Success
                ? Success(result.Data, result.Message)
                : Error(result.Message);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            var result = await _authService.ForgotPasswordAsync(email);

            return result.Success
                ? Success(result.Message)
                : Error(result.Message);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _authService.ResetPasswordAsync(
                model.Email,
                model.Code,
                model.NewPassword,
                model.ConfirmPassword
            );

            return result.Success
                ? Success(result.Message)
                : Error(result.Message);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromQuery] string userId, [FromQuery] string role)
        {
            var result = await _authService.AssignRoleAsync(userId, role);

            return result.Success
                ? Success(result.Data, result.Message)
                : Error(result.Message);
        }
    }
}
