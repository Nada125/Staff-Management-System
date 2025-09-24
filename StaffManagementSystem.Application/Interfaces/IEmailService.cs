using StaffManagementSystem.Application.DTOs.Auth;


namespace StaffManagementSystem.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto request);
    }
}
