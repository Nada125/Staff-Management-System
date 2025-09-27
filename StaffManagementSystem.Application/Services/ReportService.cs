using AutoMapper;
using StaffManagementSystem.Application.DTOs.Report;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;

namespace StaffManagementSystem.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Report, int> _reportRepo;
        private readonly IReportAiService _aiService;
        private readonly IMapper _mapper;

        public ReportService(
            IGenericRepository<Report, int> reportRepo,
            IReportAiService aiService,
            IMapper mapper)
        {
            _reportRepo = reportRepo;
            _aiService = aiService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReportResponse>> GetAllReportsAsync()
        {
            var reports = await _reportRepo.GetAll();
            return _mapper.Map<IEnumerable<ReportResponse>>(reports);
        }

        public async Task<ReportResponse?> GetReportByIdAsync(int id)
        {
            var report = await _reportRepo.Get(id);
            return report == null ? null : _mapper.Map<ReportResponse>(report);
        }

        public async Task<ReportResponse> CreateReportAsync(ReportRequest request)
        {
            var report = _mapper.Map<Report>(request);
            report.Summary = await _aiService.SummarizeAsync(report.Content);
            await _reportRepo.Add(report);
            return _mapper.Map<ReportResponse>(report);
        }

        public async Task<ReportResponse?> UpdateReportAsync(int id, ReportRequest request)
        {
            var report = await _reportRepo.Get(id);
            if (report == null) return null;

            _mapper.Map(request, report);
            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                report.Summary = await _aiService.SummarizeAsync(report.Content);
            }
            await _reportRepo.Update(report);

            return _mapper.Map<ReportResponse>(report);
        }

        public async Task<ReportResponse?> PatchReportAsync(int id, ReportRequest request)
        {
            var report = await _reportRepo.Get(id);
            if (report == null) return null;

            bool contentChanged = false;

            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                report.Content = request.Content;
                contentChanged = true;
            }

            if (!string.IsNullOrWhiteSpace(request.EmployeeId))
                report.EmployeeId = request.EmployeeId;

            if (request.SubmittedAt != default)
                report.SubmittedAt = request.SubmittedAt;

            if (contentChanged)
            {
                report.Summary = await _aiService.SummarizeAsync(report.Content);
            }

            await _reportRepo.Update(report);

            return _mapper.Map<ReportResponse>(report);
        }



        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _reportRepo.Get(id);
            if (report == null) return false;

            await _reportRepo.Delete(id);
            return true;
        }
    }
}
