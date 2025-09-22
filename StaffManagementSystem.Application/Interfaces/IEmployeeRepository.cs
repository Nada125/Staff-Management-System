using System.Threading.Tasks;
using StaffManagementSystem.Domain.Entities;

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee, string>
    {
        Task<Employee?> GetEmployeeWithTasks(string id);
    }
}
