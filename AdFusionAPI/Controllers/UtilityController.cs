using AdFusionAPI.APIConfig;
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
        private readonly ICampaignMeetingRoomService _videoCallService;

        public UtilityController(IUtilityService utilityService, ICampaignMeetingRoomService videoCallService)
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

        [HttpPost("meetingRoom")]
        [BrandRequired]
        public async Task<ActionResult<string>> CreateVideoCallRoom(RoomDataRequest dataRequest)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _videoCallService.CreateRoom(dataRequest, user);
            return Ok();
        }

        [HttpPut("meetingRoom")]
        [BrandRequired]
        public async Task<ActionResult<string>> UpdateVideoCallRoom(RoomDataUpdateRequest dataRequest)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _videoCallService.UpdateRoom(dataRequest, user);
            return Ok();
        }

        [HttpGet("meetingRoom/{name}/log")]
        public async Task<IActionResult> DownloadLogFile(string name)
        {
            var fileContent = await _videoCallService.GetLogFile(name);

            return File(fileContent.fileContent, "application/octet-stream", fileContent.fileName);
        }

        [HttpDelete("meetingRoom/{name}")]
        public async Task<ActionResult<string>> DeleteVideoRoom(string name)
        {
            await _videoCallService.DeleteRoomAsync(name);
            return Ok();
        }

        [AuthRequired]
        [HttpGet("meetingRoom/{name}/token")]
        public async Task<ActionResult<string>> GetAccessLinkByRole(string name)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var link = await _videoCallService.GetAccessLinkByRole(name, user);
            return Ok(link);
        }
    }
}