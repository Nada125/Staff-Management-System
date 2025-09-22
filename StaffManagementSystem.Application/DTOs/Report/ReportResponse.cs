using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.DTOs.Report
{
    public class ReportResponse
    {
        public int Id {  get; set; }
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string EmployeeId { get; set; } = string.Empty;
    }
}
