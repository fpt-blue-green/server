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

        public InfluencerController(IInfluencerService influencerService, IChannelService channelService, IUserService userService,  IMapper mapper)
        {
            _influencerService = influencerService;
            _channelService = channelService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [AuthRequired]
        public async Task<ActionResult<InfluencerDTO>> GetCurrentInfluencer()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetInfluencerByUserId(user.Id);
            return Ok(result);
        }

        [HttpGet("phoneNumber/validate")]
        [AuthRequired]
        public async Task<ActionResult<string>> ValidatePhoneNumber(string phoneNumber)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.ValidatePhoneNumber(user, phoneNumber);
            return Ok(result);
        }

        [HttpPut]
        [AuthRequired]
        public async Task<ActionResult<string>> CreateOrUpdateInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
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
        public async Task<ActionResult<string>> UpdateTagsForInfluencer([FromBody] List<Guid> listTags)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.UpdateTagsForInfluencer(user, listTags);
            return Ok(result);
        }

        [HttpPost("channels")]
        [InfluencerRequired]
        public async Task<ActionResult> CreateChannels([FromBody] List<ChannelPlatFormUserNameDTO> channels)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _channelService.CreateInfluencerChannel(user, channels);
            return Ok();
        }

        [HttpGet("channelUsername")]
        [InfluencerRequired]
        public async Task<ActionResult<List<ChannelDTO>>> GetChannelUserName()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _channelService.GetChannelPlatFormUserNames(user);
            return Ok(result);
        }

        [HttpPost("images")]
        [AuthRequired]
        public async Task<ActionResult<List<string>>> UploadImages(List<IFormFile> images)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _userService.UploadContentImages(images, user);
            return Ok(result);
        }
    }
}
