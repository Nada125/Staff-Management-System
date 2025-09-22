using StaffManagementSystem.Application.DTOs.Employee;

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync();
        Task<EmployeeResponse?> GetEmployeeByIdAsync(string id);
        Task<EmployeeResponse?> GetEmployeeWithTasksAsync(string id);
        Task<EmployeeResponse> CreateEmployeeAsync(EmployeeRequest request);
        Task<EmployeeResponse?> UpdateEmployeeAsync(string id, EmployeeRequest request);
        Task<EmployeeResponse?> PatchEmployeeAsync(string id, EmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(string id);
    }
}
