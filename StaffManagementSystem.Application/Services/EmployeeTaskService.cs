using AutoMapper;
using StaffManagementSystem.Application.DTOs.EmployeeTasks;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;

namespace StaffManagementSystem.Application.Services
{
    public class EmployeeTaskService : IEmployeeTaskService
    {
        private readonly IGenericRepository<EmployeeTask, int> _taskRepo;
        private readonly IMapper _mapper;

        public EmployeeTaskService(IGenericRepository<EmployeeTask, int> taskRepo, IMapper mapper)
        {
            _taskRepo = taskRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeTaskResponse>> GetAllTasksAsync()
        {
            var tasks = await _taskRepo.GetAll();
            return _mapper.Map<IEnumerable<EmployeeTaskResponse>>(tasks);
        }

        public async Task<EmployeeTaskResponse?> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepo.Get(id);
            return task == null ? null : _mapper.Map<EmployeeTaskResponse>(task);
        }

        public async Task<EmployeeTaskResponse> CreateTaskAsync(EmployeeTaskRequest request)
        {
            var task = _mapper.Map<EmployeeTask>(request);
            await _taskRepo.Add(task);
            return _mapper.Map<EmployeeTaskResponse>(task);
        }

        public async Task<EmployeeTaskResponse?> UpdateTaskAsync(int id, EmployeeTaskRequest request)
        {
            var task = await _taskRepo.Get(id);
            if (task == null) return null;

            _mapper.Map(request, task);
            await _taskRepo.Update(task);

            return _mapper.Map<EmployeeTaskResponse>(task);
        }

        public async Task<EmployeeTaskResponse?> PatchTaskAsync(int id, EmployeeTaskRequest request)
        {
            var task = await _taskRepo.Patch(id, t =>
            {
                if (!string.IsNullOrWhiteSpace(request.Title)) t.Title = request.Title;
                if (!string.IsNullOrWhiteSpace(request.Description)) t.Description = request.Description;
                if (request.AssignedDate != default) t.AssignedDate = request.AssignedDate;
                if (request.DueDate.HasValue) t.DueDate = request.DueDate;
                if (request.Status != default) t.Status = request.Status;
                if (!string.IsNullOrWhiteSpace(request.EmployeeId)) t.EmployeeId = request.EmployeeId;
            });

            return task == null ? null : _mapper.Map<EmployeeTaskResponse>(task);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _taskRepo.Get(id);
            if (task == null) return false;

            await _taskRepo.Delete(id);
            return true;
        }
    }
}
