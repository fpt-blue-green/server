using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
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
        private readonly IPackageService _packageService;

        private readonly IMapper _mapper;

        public InfluencerController(IInfluencerService influencerService, IChannelService channelService, IMapper mapper, IUserService userService, IPackageService packageSerivce)
        {
            _influencerService = influencerService;
            _channelService = channelService;
            _userService = userService;
            _mapper = mapper;
            _packageService = packageSerivce;
        }

        [HttpGet]
        [InfluencerRequired]
        public async Task<ActionResult<InfluencerDTO>> GetCurrentInfluencer()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetInfluencerByUserId(user.Id);
            return Ok(result);
        }

        [HttpPatch("phoneNumber/validate")]
        [InfluencerRequired]
        public async Task<ActionResult<string>> ValidatePhoneNumber(string phoneNumber)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.ValidatePhoneNumber(user, phoneNumber);
            return Ok(result);
        }

        [HttpPut]
        [InfluencerRequired]
        public async Task<ActionResult<string>> CreateOrUpdateInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.CreateOrUpdateInfluencer(influencerRequestDTO, user);
            return Ok(result);
        }

        [HttpGet("tags")]
        [InfluencerRequired]
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

        [HttpDelete("channels/{id}")]
        [InfluencerRequired]
        public async Task<ActionResult> DeleteChannels(Guid id)
        {
            await _channelService.DeleteInfluencerChannel(id);
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
        [InfluencerRequired]
        public async Task<ActionResult<List<string>>> UploadImages([FromForm] List<Guid> imageIds, [FromForm] List<IFormFile> images)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.UploadContentImages(imageIds, images, user, "Images");
            return Ok(result);
        }

        #region package
        [HttpPost("packages")]
        [AuthRequired]
        public async Task<ActionResult<string>> CreateInfluencerPackages([FromBody] List<PackageDTO> packages)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _packageService.CreatePackage(user.Id, packages);
            return Ok(result);
        }
        [HttpPut("package/{packageId}")]
        [InfluencerRequired]
        public async Task<ActionResult<string>> UpdateInfluencerPackage([FromBody] PackageDtoRequest package, Guid packageId)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _packageService.UpdateInfluencerPackage(user.Id, packageId, package);
            return Ok(result);
        }
        [HttpGet("packages")]
        [InfluencerRequired]
        public async Task<ActionResult<List<InfluencerDTO>>> GetInfluencerPackages()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _packageService.GetInfluPackages(user.Id);
            return Ok(result);
        }
        [HttpGet("packages/{packageId}")]
        [InfluencerRequired]
        public async Task<ActionResult<PackageDTO>> GetInfluencerPackage(Guid packageId)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _packageService.GetInfluPackage(packageId, user.Id);
            return Ok(result);
        }
        #endregion

    }
}
