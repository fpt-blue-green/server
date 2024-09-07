using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfluencerController : Controller
    {
        private readonly IInfluencerService _influencerService;
        private readonly IUserService _userService;
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;

        public InfluencerController(IInfluencerService influencerService, IChannelService channelService, IMapper mapper)
        {
            _influencerService = influencerService;
            _channelService = channelService;
            _mapper = mapper;
        }

        [HttpGet]
        [AuthRequired]
        public async Task<ActionResult<Influencer>> GetCurrentInfluencer()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetInfluencerByUserId(user.Id);
            return Ok(result);
        }

        [HttpPut]
        [AuthRequired]
        public async Task<ActionResult<Influencer>> CreateOrUpdateInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.CreateOrUpdateInfluencer(influencerRequestDTO, user);
            return Ok(result);
        }

        [HttpGet("tags")]
        [AuthRequired]
        public async Task<ActionResult<List<TagDTO>>> GetTagsByInfluencer()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetTagsByInfluencer(user);
            return Ok(result);
        }

        [HttpPost("tags")]
        [InfluencerRequired]
        public async Task<IActionResult> UpdateTagsForInfluencer([FromBody] List<Guid> listTags)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.UpdateTagsForInfluencer(user, listTags);
            return Ok(result);
        }

        [HttpPost("channels")]
        [InfluencerRequired]
        public async Task<IActionResult> CreateChannels([FromBody] List<ChannelPlatFormUserNameDTO> channels)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _channelService.CreateInfluencerChannel(user, channels);
            return Ok();
        }

        [HttpGet("channelUsername")]
        [InfluencerRequired]
        public async Task<IActionResult> GetChannelUserName()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _channelService.GetChannelPlatFormUserNames(user);
            return Ok(result);
        }

        [HttpPost("images")]
        [AuthRequired]
        public async Task<IActionResult> UploadImages(List<IFormFile> images)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var avatar = await _userService.UploadContentImages(images, user);
            return Ok(avatar);
        }
    }
}
