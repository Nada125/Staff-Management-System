using Microsoft.AspNetCore.Identity;
using StaffManagementSystem.Domain.Enums;


namespace StaffManagementSystem.Domain.Entities
{
    public class Employee : IdentityUser
    {
        public string Position { get; set; } = string.Empty;
        public string Nationality {  get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime? VerificationCodeExpiresAt { get; set; }
        public VerificationPurpose? CodePurpose { get; set; }
        public string? VerificationCode { get; set; }
        public ICollection<EmployeeTask>? Tasks { get; set; } 
        public ICollection<Report>? Reports { get; set; }
    }
}
