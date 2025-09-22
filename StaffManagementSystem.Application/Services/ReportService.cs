using AutoMapper;
using StaffManagementSystem.Application.DTOs.Report;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;

namespace StaffManagementSystem.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Report, int> _reportRepo;
        private readonly IMapper _mapper;

        public ReportService(
            IGenericRepository<Report, int> reportRepo,
            IMapper mapper)
        {
            _reportRepo = reportRepo;
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
            await _reportRepo.Add(report);
            return _mapper.Map<ReportResponse>(report);
        }

        public async Task<ReportResponse?> UpdateReportAsync(int id, ReportRequest request)
        {
            var report = await _reportRepo.Get(id);
            if (report == null) return null;

            _mapper.Map(request, report);
            await _reportRepo.Update(report);

            return _mapper.Map<ReportResponse>(report);
        }

        public async Task<ReportResponse?> PatchReportAsync(int id, ReportRequest request)
        {
            var report = await _reportRepo.Patch(id, r =>
            {
                if (!string.IsNullOrWhiteSpace(request.Summary))
                    r.Summary = request.Summary;

                if (!string.IsNullOrWhiteSpace(request.Content))
                    r.Content = request.Content;

                if (request.SubmittedAt != default)
                    r.SubmittedAt = request.SubmittedAt;

                if (!string.IsNullOrWhiteSpace(request.EmployeeId))
                    r.EmployeeId = request.EmployeeId;
            });

            return report == null ? null : _mapper.Map<ReportResponse>(report);
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
