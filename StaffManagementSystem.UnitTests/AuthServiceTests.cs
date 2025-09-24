using Moq;
using Xunit;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Application.DTOs.Response;


namespace StaffManagementSystem.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthServiceTests()
        {
            _authServiceMock = new Mock<IAuthService>();
        }


        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenRegistrationSucceeds()
        {
            // Arrange
            var request = new RegisterDto
            {
                UserName = "TestUser",
                Email = "test@example.com",
                Password = "Password123!",
            };

            var expectedResponse = new ApiResponse<object>(true, "Please enter the code we sent to your email.");

            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(expectedResponse);

            var authService = _authServiceMock.Object;

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Please enter the code we sent to your email.", result.Message);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var authDto = new AuthDto
            {
                UserId = "123",
                UserName = "TestUser",
                Email = "test@example.com",
                Token = "mocked-jwt-token",
                IsAuthenticated = true,
                Message = "Successful Login"
            };

            var expectedResponse = new ApiResponse<AuthDto>(true, "Successful Login", authDto);

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(expectedResponse);

            var authService = _authServiceMock.Object;

            // Act
            var result = await authService.LoginAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Successful Login", result.Message);
            Assert.Equal("mocked-jwt-token", result.Data!.Token);
        }
    }
}
