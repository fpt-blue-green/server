using AdFusionAPI.APIConfig;
using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStatisticController : Controller
    {
        private readonly IAdminStatisticService _adminStatistic;

        public AdminStatisticController(IAdminStatisticService adminStatistic)
        {
            _adminStatistic = adminStatistic;
        }

        [HttpGet("userActive")]
        [AdminRequired]
        public async Task<ActionResult<Dictionary<string, int>>> GetUserActive(int year = 2024, ETimeFrame times = ETimeFrame.FullYear)
        {
            var result = await _adminStatistic.GetLoginCountsByTimeFrame(year, times);
            return Ok(result);
        }

        [HttpGet("availableYear")]
        [AdminRequired]
        public ActionResult<List<int>> GetAvailableYear()
        {
            var result = _adminStatistic.GetAvailableYearInSystem();
            return Ok(result);
        }

        [HttpGet("roleCounts")]
        [AdminRequired]
        public async Task<ActionResult<List<CommomPieChartDTO>>> GetRoleCounts()
        {
            var result = await _adminStatistic.GetRoleData();
            return Ok(result);
        }

        [HttpGet("jobStatusCounts")]
        [AdminRequired]
        public async Task<ActionResult<List<CommomPieChartDTO>>> GetJobStatusCounts()
        {
            var result = await _adminStatistic.GetJobStatusData();
            return Ok(result);
        }

        [HttpGet("monthlyMetricsTrend")]
        [AdminRequired]
        public async Task<ActionResult<List<MonthlyMetricsTrendDTO>>> GetMonthlyMetricsTrend()
        {
            var result = await _adminStatistic.GetMonthlyMetricsTrend();
            return Ok(result);
        }

        [HttpGet("topFiveInfluencerUser")]
        [AdminRequired]
        public async Task<ActionResult<List<TopFiveStatisticDTO>>> GetTopFiveInfluencerUser()
        {
            var result = await _adminStatistic.GetTopFiveInfluencerUser();
            return Ok(result);
        }

        [HttpGet("topFiveBrandUser")]
        [AdminRequired]
        public async Task<ActionResult<List<TopFiveStatisticDTO>>> GetTopFiveBrandUser()
        {
            var result = await _adminStatistic.GetTopFiveBrandUser();
            return Ok(result);
        }
    }
}
