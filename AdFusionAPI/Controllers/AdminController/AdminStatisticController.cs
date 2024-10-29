using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.AdminController
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

        [HttpGet("/userActive")]
        [AdminRequired]
        public async Task<ActionResult<Dictionary<string, int>>> GetUserActive(int year, ETimeFrame times)
        {
            var result = await _adminStatistic.GetLoginCountsByTimeFrame(year, times);
            return Ok(result);
        }
    }
}
