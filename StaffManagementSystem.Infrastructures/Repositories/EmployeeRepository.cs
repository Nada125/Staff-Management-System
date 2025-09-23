using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;
using StaffManagementSystem.Infrastructures.DBContext;
using StaffManagementSystem.Infrastructures.Repositories;

public class EmployeeRepository : GenericRepository<Employee, string>, IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Employee?> GetEmployeeWithTasks(string id)
    {
        return await _context.Employees
            .Include(e => e.Tasks)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
