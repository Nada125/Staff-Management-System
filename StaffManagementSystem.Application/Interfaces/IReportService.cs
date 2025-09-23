using StaffManagementSystem.Application.DTOs.Report;

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReportResponse>> GetAllReportsAsync();
        Task<ReportResponse?> GetReportByIdAsync(int id);
        Task<ReportResponse> CreateReportAsync(ReportRequest request);
        Task<ReportResponse?> UpdateReportAsync(int id, ReportRequest request);
        Task<ReportResponse?> PatchReportAsync(int id, ReportRequest request);
        Task<bool> DeleteReportAsync(int id);
    }
}
