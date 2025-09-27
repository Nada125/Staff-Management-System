using StaffManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.DTOs.Auth
{
    public class VerifyCodeDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification code is required")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification purpose is required")]
        [EnumDataType(typeof(VerificationPurpose), ErrorMessage = "Invalid verification purpose")]
        public VerificationPurpose Purpose { get; set; }
    }
}
