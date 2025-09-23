using AutoMapper;
using StaffManagementSystem.Application.DTOs.Employee;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepo, IMapper mapper)
    {
        _employeeRepo = employeeRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepo.GetAll();
        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }

    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(string id)
    {
        var employee = await _employeeRepo.Get(id);
        return employee == null ? null : _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeResponse?> GetEmployeeWithTasksAsync(string id)
    {
        var employee = await _employeeRepo.GetEmployeeWithTasks(id);
        return employee == null ? null : _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeResponse> CreateEmployeeAsync(EmployeeRequest request)
    {
        var employee = _mapper.Map<Employee>(request);
        await _employeeRepo.Add(employee);
        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeResponse?> UpdateEmployeeAsync(string id, EmployeeRequest request)
    {
        var employee = await _employeeRepo.Get(id);
        if (employee == null) return null;

        _mapper.Map(request, employee);
        await _employeeRepo.Update(employee);

        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeResponse?> PatchEmployeeAsync(string id, EmployeeRequest request)
    {
        var employee = await _employeeRepo.Patch(id, e =>
        {
            if (!string.IsNullOrWhiteSpace(request.UserName)) e.UserName = request.UserName;
            if (!string.IsNullOrWhiteSpace(request.Email)) e.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) e.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(request.Position)) e.Position = request.Position;
            if (request.Salary > 0) e.Salary = request.Salary;
        });

        return employee == null ? null : _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<bool> DeleteEmployeeAsync(string id)
    {
        var employee = await _employeeRepo.Get(id);
        if (employee == null) return false;

        await _employeeRepo.Delete(id);
        return true;
    }
}
