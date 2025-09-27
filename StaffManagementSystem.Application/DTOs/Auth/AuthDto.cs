using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.DTOs.Auth
{
    public class AuthDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public List<string>? Roles { get; set; }
    }
}
