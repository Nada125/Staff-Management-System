using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Domain.Entities
{
    public class EmployeeTask : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public Employee Employee { get; set; } = null!; 
    }
}
