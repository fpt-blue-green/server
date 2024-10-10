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
        private readonly IVideoCallService _videoCallService;

        public UtilityController(IUtilityService utilityService, IVideoCallService videoCallService)
        {
            this._utilityService = utilityService;
            this._videoCallService = videoCallService;
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

        [HttpPost("videoRoom/{name}")]
        public async Task<ActionResult<string>> CreateVideoCallRoom(string name)
        {
            var info = await _videoCallService.CreateRoom(name);
            return Ok(info);
        }

        [HttpGet("videoRoom/log")]
        public async Task<IActionResult> DownloadLogFile()
        {
            var fileContent = await _videoCallService.GetLogFile();

            return File(fileContent.fileContent, "application/octet-stream", fileContent.fileName);
        }

        [HttpDelete("videoRoom/{name}")]
        public async Task<ActionResult<string>> DeleteVideoRoom(string name)
        {
            await _videoCallService.DeleteRoomAsync(name);
            return Ok();
        }
    }
}