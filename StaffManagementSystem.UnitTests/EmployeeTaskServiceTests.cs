using AutoMapper;
using Moq;
using StaffManagementSystem.Application.DTOs.EmployeeTasks;
using StaffManagementSystem.Application.Services;
using StaffManagementSystem.Domain.Entities;
using StaffManagementSystem.Application.Interfaces;
using Xunit;

namespace StaffManagementSystem.UnitTests
{
    public class EmployeeTaskServiceTests
    {
        private readonly Mock<IGenericRepository<EmployeeTask, int>> _taskRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EmployeeTaskService _taskService;

        public EmployeeTaskServiceTests()
        {
            _taskRepoMock = new Mock<IGenericRepository<EmployeeTask, int>>();
            _mapperMock = new Mock<IMapper>();
            _taskService = new EmployeeTaskService(_taskRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllTasksAsync_ReturnsMappedTasks()
        {
            // Arrange
            var tasks = new List<EmployeeTask>
            {
                new EmployeeTask { Id = 1, Title = "Task1" },
                new EmployeeTask { Id = 2, Title = "Task2" }
            };

            _taskRepoMock.Setup(r => r.GetAll()).ReturnsAsync(tasks);
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeTaskResponse>>(tasks))
                       .Returns(tasks.Select(t => new EmployeeTaskResponse { Id = t.Id, Title = t.Title }));

            // Act
            var result = await _taskService.GetAllTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Title == "Task1");
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenFound()
        {
            // Arrange
            var task = new EmployeeTask { Id = 1, Title = "Task1" };

            _taskRepoMock.Setup(r => r.Get(1)).ReturnsAsync(task);
            _mapperMock.Setup(m => m.Map<EmployeeTaskResponse>(task))
                       .Returns(new EmployeeTaskResponse { Id = 1, Title = "Task1" });

            // Act
            var result = await _taskService.GetTaskByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Task1", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _taskRepoMock.Setup(r => r.Get(99)).ReturnsAsync((EmployeeTask?)null);

            // Act
            var result = await _taskService.GetTaskByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTaskAsync_CreatesAndReturnsTask()
        {
            // Arrange
            var request = new EmployeeTaskRequest { Title = "NewTask" };
            var task = new EmployeeTask { Id = 123, Title = "NewTask" };
            var response = new EmployeeTaskResponse { Id = 123, Title = "NewTask" };

            _mapperMock.Setup(m => m.Map<EmployeeTask>(request)).Returns(task);
            _taskRepoMock.Setup(r => r.Add(It.IsAny<EmployeeTask>())).ReturnsAsync((EmployeeTask t) => t);
            _mapperMock.Setup(m => m.Map<EmployeeTaskResponse>(task)).Returns(response);

            // Act
            var result = await _taskService.CreateTaskAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewTask", result.Title);
            _taskRepoMock.Verify(r => r.Add(task), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_UpdatesAndReturnsTask_WhenFound()
        {
            // Arrange
            var existingTask = new EmployeeTask { Id = 1, Title = "OldTask" };
            var request = new EmployeeTaskRequest { Title = "UpdatedTask" };

            _taskRepoMock.Setup(r => r.Get(1)).ReturnsAsync(existingTask);
            _mapperMock.Setup(m => m.Map(request, existingTask));
            _taskRepoMock.Setup(r => r.Update(existingTask)).ReturnsAsync(existingTask);
            _mapperMock.Setup(m => m.Map<EmployeeTaskResponse>(existingTask))
                       .Returns(new EmployeeTaskResponse { Id = 1, Title = "UpdatedTask" });

            // Act
            var result = await _taskService.UpdateTaskAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UpdatedTask", result.Title);
            _taskRepoMock.Verify(r => r.Update(existingTask), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var request = new EmployeeTaskRequest { Title = "UpdatedTask" };
            _taskRepoMock.Setup(r => r.Get(99)).ReturnsAsync((EmployeeTask?)null);

            // Act
            var result = await _taskService.UpdateTaskAsync(99, request);

            // Assert
            Assert.Null(result);
            _taskRepoMock.Verify(r => r.Update(It.IsAny<EmployeeTask>()), Times.Never);
        }

        [Fact]
        public async Task PatchTaskAsync_PatchesAndReturnsTask_WhenFound()
        {
            // Arrange
            var patchedTask = new EmployeeTask { Id = 1, Title = "PatchedTask" };

            _taskRepoMock.Setup(r => r.Patch(1, It.IsAny<Action<EmployeeTask>>()))
                         .ReturnsAsync(patchedTask);
            _mapperMock.Setup(m => m.Map<EmployeeTaskResponse>(patchedTask))
                       .Returns(new EmployeeTaskResponse { Id = 1, Title = "PatchedTask" });

            // Act
            var result = await _taskService.PatchTaskAsync(1, new EmployeeTaskRequest { Title = "PatchedTask" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PatchedTask", result.Title);
        }

        [Fact]
        public async Task PatchTaskAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _taskRepoMock.Setup(r => r.Patch(99, It.IsAny<Action<EmployeeTask>>()))
                         .ReturnsAsync((EmployeeTask?)null);

            // Act
            var result = await _taskService.PatchTaskAsync(99, new EmployeeTaskRequest { Title = "NoTask" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ReturnsTrue_WhenTaskExists()
        {
            // Arrange
            var task = new EmployeeTask { Id = 1, Title = "Task1" };
            _taskRepoMock.Setup(r => r.Get(1)).ReturnsAsync(task);
            _taskRepoMock.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.DeleteTaskAsync(1);

            // Assert
            Assert.True(result);
            _taskRepoMock.Verify(r => r.Delete(1), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ReturnsFalse_WhenTaskNotFound()
        {
            // Arrange
            _taskRepoMock.Setup(r => r.Get(99)).ReturnsAsync((EmployeeTask?)null);

            // Act
            var result = await _taskService.DeleteTaskAsync(99);

            // Assert
            Assert.False(result);
            _taskRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }
    }
}
