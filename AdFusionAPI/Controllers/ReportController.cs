using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [AdminRequired]
        public async Task<ActionResult<FilterListResponse<ReportDTO>>> GetAll([FromQuery] ReportFilterDTO filter)
        {
            var report = await _reportService.GetReports(filter);
            return Ok(report);
        }

        [HttpPut("{id}/reject")]
        [AdminRequired]
        public async Task<ActionResult> RejectReport(Guid id)
        {
            await _reportService.RejectReport(id);
            return Ok();
        }

        [HttpPut("{id}/approve")]
        [AdminRequired]
        public async Task<ActionResult> ApproveReport(Guid id, [FromBody] BannedUserRequestDTO bannedUserRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _reportService.ApproveReport(id, user, bannedUserRequestDTO);
            return Ok();
        }
    }
}
