using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

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

        [HttpGet("location")]
        public ActionResult<string> GetLocation(string keyName)
        {
            var cities = _utilityService.GetCitiesWithCountry(keyName);

            if (cities == null)
            {
                return NotFound();
            }

            return Ok(cities);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ChannelStatDTO>> GetChannelProfile([FromQuery] int platform, [FromQuery] string channelId)
        {
            var info = await _utilityService.GetChannelProfile(platform, channelId);
            return Ok(info);
        }

        [HttpGet("video")]
        public async Task<ActionResult<string>> GetVideoInformation(int platform, string url)
        {
            var info = await _utilityService.GetVideoInformation(platform, url);
            return Ok(info);
        }
    }
}