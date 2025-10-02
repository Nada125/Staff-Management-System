using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.DTOs.Report;
using StaffManagementSystem.Application.Interfaces;

namespace StaffManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IReportAiService _aiService;

        public ReportController(IReportService reportService, IReportAiService aiService)
        {
            _reportService = reportService;
            _aiService = aiService;
        }

        //[Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _reportService.GetAllReportsAsync();
            return Success(response, "Reports retrieved successfully");
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _reportService.GetReportByIdAsync(id);
            if (response == null)
                return Error("Report not found", 404);

            return Success(response, "Report retrieved successfully");
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReportRequest request)
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

            var response = await _reportService.CreateReportAsync(request);

            var summary = await _aiService.SummarizeAsync(request.Content);

            return Success(new {response}, "Report created successfully with AI summary");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReportRequest request)
        {
            var response = await _reportService.UpdateReportAsync(id, request);
            if (response == null)
                return Error("Report not found", 404);

            return Success(response, "Report updated successfully");
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] ReportRequest request)
        {
            var response = await _reportService.PatchReportAsync(id, request);
            if (response == null)
                return Error("Report not found", 404);

            return Success(response, "Report patched successfully");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _reportService.DeleteReportAsync(id);
            if (!deleted)
                return Error("Report not found", 404);

            return Success<string>(null!, "Report deleted successfully");
        }
    }
}
