using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.API.Controllers;
using StaffManagementSystem.Application.DTOs.Employee;
using StaffManagementSystem.Application.Interfaces;


[ApiController]
[Route("api/[controller]")]
public class EmployeeController : BaseController
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }


    //[Authorize(Roles ="Manager")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Success(employees, "Employees retrieved successfully");
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        return employee == null ? Error("Employee not found", 404) : Success(employee, "Employee retrieved successfully");
    }


    [Authorize]
    [HttpGet("Task/{id}")]
    public async Task<IActionResult> GetEmployeeWithTasks(string id)
    {
        var employee = await _employeeService.GetEmployeeWithTasksAsync(id);
        return employee == null ? Error("Employee not found", 404) : Success(employee, "Employee retrieved successfully");
    }


    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(ms => ms.Value!.Errors.Count > 0)
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return ValidationError(errors);
        }

        var response = await _employeeService.CreateEmployeeAsync(request);
        return Success(response, "Employee created successfully");
    }


    [Authorize(Roles = "Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] EmployeeRequest request)
    {
        var response = await _employeeService.UpdateEmployeeAsync(id, request);
        return response == null ? Error("Employee not found", 404) : Success(response, "Employee updated successfully");
    }


    [Authorize(Roles = "Manager")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] EmployeeRequest request)
    {
        var response = await _employeeService.PatchEmployeeAsync(id, request);
        return response == null ? Error("Employee not found", 404) : Success(response, "Employee patched successfully");
    }


    [Authorize(Roles = "Manager")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        return deleted ? Success<string>(null!, "Employee deleted successfully") : Error("Employee not found", 404);
    }
}
