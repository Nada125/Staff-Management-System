using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Domain.Entities
{
    public class Employee : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public ICollection<EmployeeTask>? Tasks { get; set; } 
        public ICollection<Report>? Reports { get; set; }
    }
}
