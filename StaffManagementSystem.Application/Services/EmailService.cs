using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.Interfaces;

namespace StaffManagementSystem.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly Email _emailConfig;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<Email> emailConfig, ILogger<EmailService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailDto request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emailConfig.User));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Plain) { Text = request.Body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailConfig.Host, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailConfig.User, _emailConfig.Pass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", request.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", request.To);
                throw;
            }
        }
    }
}
