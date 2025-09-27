using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.DTOs.Response;
using StaffManagementSystem.Domain.Enums;


namespace StaffManagementSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<AuthDto>> LoginAsync(LoginDto model);
        Task<ApiResponse<string>> VerifyCodeAsync(string email, string code, VerificationPurpose purpose);
        Task<ApiResponse<object>> ForgotPasswordAsync(string email);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string code, string newPassword, string confirmPassword);

        Task<ApiResponse<string>> AssignRoleAsync(string userId, string newRole);
    }
}
