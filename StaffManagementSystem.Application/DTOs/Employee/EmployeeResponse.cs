using StaffManagementSystem.Application.DTOs.EmployeeTasks;
namespace StaffManagementSystem.Application.DTOs.Employee
{
    public class EmployeeResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public ICollection<EmployeeTaskResponse>? Tasks { get; set; }

    }
}
