using StaffManagementSystem.Application.DTOs.EmployeeTasks;

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IEmployeeTaskService
    {
        Task<IEnumerable<EmployeeTaskResponse>> GetAllTasksAsync();
        Task<EmployeeTaskResponse?> GetTaskByIdAsync(int id);
        Task<EmployeeTaskResponse> CreateTaskAsync(EmployeeTaskRequest request);
        Task<EmployeeTaskResponse?> UpdateTaskAsync(int id, EmployeeTaskRequest request);
        Task<EmployeeTaskResponse?> PatchTaskAsync(int id, EmployeeTaskRequest request);
        Task<bool> DeleteTaskAsync(int id);
    }
}
