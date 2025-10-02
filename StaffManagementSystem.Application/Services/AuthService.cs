using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.DTOs.Response;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;
using StaffManagementSystem.Domain.Enums;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace StaffManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Employee> _userManager;
        private readonly JWT _jwt;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<Employee> userManager, IOptions<JWT> jwt, IEmailService emailService)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _emailService = emailService;
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return new ApiResponse<object>(false, "Email and Password are required.");

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new ApiResponse<object>(false, "This email is already in use!");

           
            var user = new Employee
            {
                Email = dto.Email,
                UserName = dto.UserName,
                PhoneNumber = dto.PhoneNumber,
                NationalId = dto.NationalId,
                Nationality = dto.Nationality,
                EmailConfirmed = false,
                CodePurpose = VerificationPurpose.EmailVerification
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new ApiResponse<object>(false, string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Employee");

            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            user.VerificationCode = code;
            user.VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(1);
            await _userManager.UpdateAsync(user);

            try
            {
                await _emailService.SendEmailAsync(new EmailDto
                {
                    To = user.Email!,
                    Subject = "Your verification code",
                    Body = $"Your verification code is: {code}"
                });
            }
            catch
            {
                return new ApiResponse<object>(false, "User registered, but failed to send verification email.");
            }

            return new ApiResponse<object>(true, "Please enter the code we sent to your email.", new
            {
                userId = user.Id,
                email = user.Email,
                code = code
            });
        }

        public async Task<ApiResponse<string>> VerifyCodeAsync(string email, string code, VerificationPurpose purpose)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponse<string>(false, "User not found.");

            if (user.VerificationCode != code ||
                user.VerificationCodeExpiresAt == null ||
                user.VerificationCodeExpiresAt < DateTime.UtcNow ||
                user.CodePurpose != purpose)
            {
                return new ApiResponse<string>(false, "Invalid or expired verification code.");
            }

            if (purpose == VerificationPurpose.EmailVerification)
                user.EmailConfirmed = true;

            if (user.CodePurpose == VerificationPurpose.EmailVerification)
            {
                user.VerificationCode = null;
                user.VerificationCodeExpiresAt = null;
                user.CodePurpose = null;
            }

            await _userManager.UpdateAsync(user);

            return new ApiResponse<string>(true, "Successful Verification.");
        }

        public async Task<ApiResponse<AuthDto>> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new ApiResponse<AuthDto>(false, "Email or Password is incorrect!");

            if (!user.EmailConfirmed)
                return new ApiResponse<AuthDto>(false, "Please verify your email before logging in.");

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var authDto = new AuthDto
            {
                UserId = user.Id,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                Email = user.Email!,
                Nationality= user.Nationality,
                NationalId= user.NationalId,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpireAt = jwtSecurityToken.ValidTo,
                Roles = roles.ToList(),
                Message = "Successful Login"
            };

            return new ApiResponse<AuthDto>(true, "Successful Login", authDto);
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponse<object>(false, "No account found with this email.");

            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            user.VerificationCode = code;
            user.VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(15);
            user.CodePurpose = VerificationPurpose.PasswordReset;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new ApiResponse<object>(false, string.Join(", ", updateResult.Errors.Select(e => e.Description)));

            try
            {
                await _emailService.SendEmailAsync(new EmailDto
                {
                    To = user.Email!,
                    Subject = "Password Reset Code",
                    Body = $"Use this code to reset your password: {code}"
                });
            }
            catch
            {
                return new ApiResponse<object>(false, "Failed to send password reset email.");
            }

            return new ApiResponse<object>(true, "Code sent to your email.", new
            {
                userId = user.Id,
                email = user.Email,
                code = code
            });
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(string email, string code, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                return new ApiResponse<string>(false, "Passwords do not match.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponse<string>(false, "User not found.");

            if (user.VerificationCode != code ||
                user.VerificationCodeExpiresAt == null ||
                user.VerificationCodeExpiresAt < DateTime.UtcNow ||
                user.CodePurpose != VerificationPurpose.PasswordReset)
            {
                return new ApiResponse<string>(false, "Invalid or expired verification code.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                return new ApiResponse<string>(false, string.Join("; ", result.Errors.Select(e => e.Description)));

            user.VerificationCode = null;
            user.VerificationCodeExpiresAt = null;
            user.CodePurpose = null;
            await _userManager.UpdateAsync(user);

            return new ApiResponse<string>(true, "Password reset successfully. You can now log in.");
        }

        public async Task<ApiResponse<string>> AssignRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<string>(false, "User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return new ApiResponse<string>(false, "Failed to remove existing roles");

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
                return new ApiResponse<string>(false, "Failed to assign new role");

            return new ApiResponse<string>(true, "Role assigned successfully", newRole);
        }

        private async Task<JwtSecurityToken> CreateJwtToken(Employee user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: credentials
            );
        }
    }
}
