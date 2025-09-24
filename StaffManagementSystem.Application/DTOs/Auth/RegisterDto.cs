using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "User name is required")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters")]
        public string UserName { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{5,15}$", ErrorMessage = "Enter a valid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Nationality is required")]
        public string Nationality { get; set; } = string.Empty;


        [Required(ErrorMessage = "National ID is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#\$&*~%^]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters, include uppercase, lowercase, and special character")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
