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
        private readonly IPackageService _packageService;
        private readonly IJobService _jobService;

        private readonly IMapper _mapper;

        public InfluencerController(IInfluencerService influencerService, IChannelService channelService, IMapper mapper,
                                    IUserService userService, IPackageService packageSerivce, IJobService jobService)
        {
            _influencerService = influencerService;
            _channelService = channelService;
            _userService = userService;
            _mapper = mapper;
            _packageService = packageSerivce;
            _jobService = jobService;
        }

        [HttpGet]
        [InfluencerRequired]
        public async Task<ActionResult<InfluencerDTO>> GetCurrentInfluencer()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetInfluencerByUserId(user.Id);
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
            var result = await _influencerService.UploadContentImages(imageIds, images, user, "InfluencerImages");
            return Ok(result);
        }

        [HttpPost("phone/sendOtp")]
        [InfluencerRequired]
        public async Task<ActionResult<bool>> SendPhoneOtp([FromBody] SendPhoneDTO body)
        {
            var result = await _influencerService.SendPhoneOtp(body.Phone);
            return Ok(result);
        }

        [HttpPost("phone/verify")]
        [InfluencerRequired]
        public async Task<ActionResult<bool>> VerifyOtp([FromBody] VerifyPhoneDTO body)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.VerifyPhoneOtp(user, body.Phone, body.OTP);
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
        public async Task<ActionResult<List<PackageDTO>>> GetInfluencerPackages()
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

        [HttpGet("loginHistory")]
        [AuthRequired]
        public async Task<ActionResult<IEnumerable<UserDeviceDTO>>> GetInfluencerLoginHistory()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetInfluencerLoginHistory(user);
            return Ok(result);

        }

        [HttpGet("jobs")]
        [InfluencerRequired]
        public async Task<ActionResult<List<JobDTO>>> GetJobs()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _jobService.GetAllJobByCurrentAccount(user);
            return Ok(result);
        }

        [HttpGet("jobs/filter")]
        [InfluencerRequired]
        public async Task<ActionResult<List<JobDTO>>> FilterJobByJobStatus(int? eJobStatus, int? eCampaignStatus)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _jobService.FilterJob(user, eJobStatus, eCampaignStatus);
            return Ok(result);
        }
    }
}
