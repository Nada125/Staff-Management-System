using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.DTOs.Auth
{
    public class GoogleAiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-1.5-flash";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
        public double Temperature { get; set; } = 0.7;
        public int MaxOutputTokens { get; set; } = 512;
    }
}
