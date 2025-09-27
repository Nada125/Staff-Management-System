using StaffManagementSystem.Application.DTOs.Employee;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;
using AutoMapper;
using Moq;
using Xunit;
using System;


namespace StaffManagementSystem.UnitTests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _employeeService = new EmployeeService(_employeeRepoMock.Object, _mapperMock.Object);
        }


        [Fact]
        public async Task GetAllEmployeesAsync_ReturnsMappedEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", UserName = "John", Email = "john@test.com" },
                new Employee { Id = "2", UserName = "Jane", Email = "jane@test.com" }
            };

            _employeeRepoMock.Setup(r => r.GetAll()).ReturnsAsync(employees);
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeResponse>>(employees))
                       .Returns(employees.Select(e => new EmployeeResponse { Id = e.Id, UserName = e.UserName! }));

            // Act
            var result = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.UserName == "John");
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsEmployee_WhenFound()
        {
            // Arrange
            var employee = new Employee { Id = "1", UserName = "John" };

            _employeeRepoMock.Setup(r => r.Get("1")).ReturnsAsync(employee);
            _mapperMock.Setup(m => m.Map<EmployeeResponse>(employee))
                       .Returns(new EmployeeResponse { Id = "1", UserName = "John" });

            // Act
            var result = await _employeeService.GetEmployeeByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.UserName);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _employeeRepoMock.Setup(r => r.Get("99")).ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.GetEmployeeByIdAsync("99");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateEmployeeAsync_CreatesAndReturnsEmployee()
        {
            // Arrange
            var request = new EmployeeRequest { UserName = "NewUser", Email = "new@test.com" };
            var employee = new Employee { Id = "123", UserName = "NewUser" };
            var response = new EmployeeResponse { Id = "123", UserName = "NewUser" };

            _mapperMock.Setup(m => m.Map<Employee>(request)).Returns(employee);
            _employeeRepoMock.Setup(r => r.Get("1"))
                 .ReturnsAsync(new Employee { Id = "1", UserName = "John" });
            _mapperMock.Setup(m => m.Map<EmployeeResponse>(employee)).Returns(response);

            // Act
            var result = await _employeeService.CreateEmployeeAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewUser", result.UserName);
            _employeeRepoMock.Verify(r => r.Add(employee), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ReturnsTrue_WhenEmployeeExists()
        {
            // Arrange
            var employee = new Employee { Id = "1", UserName = "John" };
            _employeeRepoMock.Setup(r => r.Get("1")).ReturnsAsync(employee);
            _employeeRepoMock.Setup(r => r.Delete("1")).Returns(Task.CompletedTask);

            // Act
            var result = await _employeeService.DeleteEmployeeAsync("1");

            // Assert
            Assert.True(result);
            _employeeRepoMock.Verify(r => r.Delete("1"), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ReturnsFalse_WhenEmployeeNotFound()
        {
            // Arrange
            _employeeRepoMock.Setup(r => r.Get("99")).ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.DeleteEmployeeAsync("99");

            // Assert
            Assert.False(result);
            _employeeRepoMock.Verify(r => r.Delete(It.IsAny<string>()), Times.Never);
        }
    }
}
