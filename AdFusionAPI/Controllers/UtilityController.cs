using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : Controller
    {
        private readonly IUtilityService _utilityService;

        public UtilityController(IUtilityService utilityService)
        {
            this._utilityService = utilityService;
        }

        [HttpGet("location/{keyName}")]
        public async Task<IActionResult> GetLocation(string keyName)
        {
            var cities = _utilityService.GetCitiesWithCountry(keyName);

            if (cities == null)
            {
                return NotFound();
            }

            return Ok(cities);
        }

        [HttpGet("tiktok/info/{url}")]
        public async Task<IActionResult> GetTikTokInformation(string url)
        {
            var info = await _utilityService.GetTikTokInformation(url);
            if (info == null)
            {
                return NotFound();
            }

            return Ok(info);
        }

    }
}
