using AutoMapper;
using Moq;
using StaffManagementSystem.Application.DTOs.Report;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Application.Services;
using StaffManagementSystem.Domain.Entities;
using Xunit;

namespace StaffManagementSystem.UnitTests
{
    public class ReportServiceTests
    {
        private readonly Mock<IGenericRepository<Report, int>> _reportRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IReportAiService> _aiServiceMock;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _reportRepoMock = new Mock<IGenericRepository<Report, int>>();
            _mapperMock = new Mock<IMapper>();
            _aiServiceMock = new Mock<IReportAiService>();

            _reportService = new ReportService(
                _reportRepoMock.Object,
                _aiServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetAllReportsAsync_ReturnsMappedReports()
        {
            // Arrange
            var reports = new List<Report> { new Report { Id = 1, Summary = "Test" } };
            var response = new List<ReportResponse> { new ReportResponse { Id = 1, Summary = "Test" } };

            _reportRepoMock.Setup(r => r.GetAll()).ReturnsAsync(reports);
            _mapperMock.Setup(m => m.Map<IEnumerable<ReportResponse>>(reports)).Returns(response);

            // Act
            var result = await _reportService.GetAllReportsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test", result.First().Summary);
        }

        [Fact]
        public async Task GetReportByIdAsync_WhenFound_ReturnsMappedReport()
        {
            // Arrange
            var report = new Report { Id = 1, Summary = "Test" };
            var response = new ReportResponse { Id = 1, Summary = "Test" };

            _reportRepoMock.Setup(r => r.Get(1)).ReturnsAsync(report);
            _mapperMock.Setup(m => m.Map<ReportResponse>(report)).Returns(response);

            // Act
            var result = await _reportService.GetReportByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Summary);
        }

        [Fact]
        public async Task GetReportByIdAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _reportRepoMock.Setup(r => r.Get(1)).ReturnsAsync((Report?)null);

            // Act
            var result = await _reportService.GetReportByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateReportAsync_AddsReportAndReturnsMappedResponse()
        {
            // Arrange
            var request = new ReportRequest { Content = "This is a report." };
            var report = new Report { Id = 1, Content = request.Content, Summary = "Summarized" };
            var response = new ReportResponse { Id = 1, Summary = "Summarized" };

            _mapperMock.Setup(m => m.Map<Report>(request)).Returns(report);
            _reportRepoMock.Setup(r => r.Add(report)).ReturnsAsync(report);
            _mapperMock.Setup(m => m.Map<ReportResponse>(report)).Returns(response);

            // Act
            var result = await _reportService.CreateReportAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Summarized", result.Summary);
        }

        [Fact]
        public async Task UpdateReportAsync_WhenFound_UpdatesAndReturnsMappedResponse()
        {
            // Arrange
            var request = new ReportRequest { Content = "This is a report." };
            var report = new Report { Id = 1, Summary = "Old" };
            var response = new ReportResponse { Id = 1, Summary = "Updated" };

            _reportRepoMock.Setup(r => r.Get(1)).ReturnsAsync(report);
            _mapperMock.Setup(m => m.Map(request, report));
            _reportRepoMock.Setup(r => r.Update(report)).ReturnsAsync(report);
            _mapperMock.Setup(m => m.Map<ReportResponse>(report)).Returns(response);

            // Act
            var result = await _reportService.UpdateReportAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Summary);
        }

        [Fact]
        public async Task UpdateReportAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _reportRepoMock.Setup(r => r.Get(1)).ReturnsAsync((Report?)null);

            // Act
            var result = await _reportService.UpdateReportAsync(1, new ReportRequest());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task PatchReportAsync_WhenFoundAndContentChanged_UpdatesContentAndSummary()
        {
            // Arrange
            var request = new ReportRequest { Content = "This is a new report content." };
            var report = new Report { Id = 1, Summary = "Old Summary", Content = "Old content" };
            var updatedSummary = "AI-generated summary for new content";
            var response = new ReportResponse { Id = 1, Content = request.Content, Summary = updatedSummary };

            _reportRepoMock
                .Setup(r => r.Get(1))
                .ReturnsAsync(report);

            _aiServiceMock
                .Setup(a => a.SummarizeAsync(request.Content, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedSummary);

            _reportRepoMock
                .Setup(r => r.Update(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            _mapperMock
                .Setup(m => m.Map<ReportResponse>(It.IsAny<Report>()))
                .Returns((Report r) => new ReportResponse
                {
                    Id = r.Id,
                    Content = r.Content,
                    Summary = r.Summary
                });

            // Act
            var result = await _reportService.PatchReportAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Content, result.Content);
            Assert.Equal(updatedSummary, result.Summary);

            _reportRepoMock.Verify(r => r.Get(1), Times.Once);
            _aiServiceMock.Verify(a => a.SummarizeAsync(request.Content, It.IsAny<CancellationToken>()), Times.Once);
            _reportRepoMock.Verify(r => r.Update(It.IsAny<Report>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ReportResponse>(It.IsAny<Report>()), Times.Once);
        }




        [Fact]
        public async Task PatchReportAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _reportRepoMock.Setup(r => r.Patch(1, It.IsAny<Action<Report>>()))
                           .ReturnsAsync((Report?)null);

            // Act
            var result = await _reportService.PatchReportAsync(1, new ReportRequest());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteReportAsync_WhenFound_DeletesAndReturnsTrue()
        {
            // Arrange
            var report = new Report { Id = 1 };
            _reportRepoMock.Setup(r => r.Get(1)).ReturnsAsync(report);
            _reportRepoMock.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _reportService.DeleteReportAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteReportAsync_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            _reportRepoMock.Setup(r => r.Get(1)).ReturnsAsync((Report?)null);

            // Act
            var result = await _reportService.DeleteReportAsync(1);

            // Assert
            Assert.False(result);
        }
    }
}
