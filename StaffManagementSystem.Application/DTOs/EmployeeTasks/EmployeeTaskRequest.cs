using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.DTOs.EmployeeTasks
{
    public class EmployeeTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
    }
}
