using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string EmployeeId { get; set; } = string.Empty;
        public Employee Employee { get; set; } = null!; 
    }
}
