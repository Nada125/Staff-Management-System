using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.DTOs.EmployeeTasks;
using StaffManagementSystem.Application.Interfaces;

namespace StaffManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTaskController : BaseController
    {
        private readonly IEmployeeTaskService _taskService;

        public EmployeeTaskController(IEmployeeTaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Success(tasks, "Tasks retrieved successfully");
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return task == null ? Error("Task not found", 404) : Success(task, "Task retrieved successfully");
        }


        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeTaskRequest request)
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

            var response = await _taskService.CreateTaskAsync(request);
            return Success(response, "Task created successfully");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeTaskRequest request)
        {
            var response = await _taskService.UpdateTaskAsync(id, request);
            return response == null ? Error("Task not found", 404) : Success(response, "Task updated successfully");
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] EmployeeTaskRequest request)
        {
            var response = await _taskService.PatchTaskAsync(id, request);
            return response == null ? Error("Task not found", 404) : Success(response, "Task patched successfully");
        }


        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            return deleted ? Success<string>(null!, "Task deleted successfully") : Error("Task not found", 404);
        }
    }
}
