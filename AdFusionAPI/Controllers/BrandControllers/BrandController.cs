using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.BrandControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly ICampaignService _campaignService;
        private readonly IFavoriteService _favoriteService;
        private readonly IJobService _jobService;
        private readonly IPaymentService _paymentService;

        public BrandController(IBrandService brandService, ICampaignService campaignService, IFavoriteService favoriteService, IJobService jobService, IPaymentService paymentService)
        {
            _brandService = brandService;
            _campaignService = campaignService;
            _favoriteService = favoriteService;
            _jobService = jobService;
            _paymentService = paymentService;
        }

        [HttpGet]
        [BrandRequired]
        public async Task<ActionResult<BrandDTO>> GetCurrentBrand()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.GetBrandByUserId(user.Id);
            return Ok(result);
        }

        [HttpPatch("CoverImg")]
        [BrandRequired]
        public async Task<ActionResult<string>> UpdateCoverImg(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var coverImg = await _brandService.UploadCoverImgAsync(file, "Cover", user);
            return Ok(coverImg);
        }

        [HttpPut]
        [BrandRequired]
        public async Task<ActionResult<string>> CreateOrUpdateBrand([FromBody] BrandRequestDTO brandRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.CreateOrUpdateBrand(brandRequestDTO, user);
            return Ok(result);
        }

        [HttpPut("social")]
        [BrandRequired]
        public async Task<ActionResult<string>> UpdateBrandSocial([FromBody] BrandSocialDTO brandSocialDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.UpdateBrandSocial(brandSocialDTO, user);
            return Ok(result);
        }

        [HttpGet("campaigns")]
        [AuthRequired]
        public async Task<ActionResult<FilterListResponse<CampaignDTO>>> GetBrandCampaigns([FromQuery] BrandCampaignFilterDTO filter)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.GetBrandCampaignsByUserId(user.Id, filter);
            return Ok(result);
        }

        #region Favorite
        [HttpGet("favorites")]
        [BrandRequired]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetAllFavoriteByBrandId()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _favoriteService.GetAllFavorites(user);
            return Ok(result);
        }

        [HttpPost("favorites/{id}")]
        [BrandRequired]
        public async Task<ActionResult> CreateFavorite(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _favoriteService.CreateFavorite(id, user);
            return Ok();
        }

        [HttpDelete("favorites/influencer/{id}")]
        [BrandRequired]
        public async Task<ActionResult> DeleteFavoriteByInfluencerId(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _favoriteService.DeleteFavoriteByInfluencerId(id, user);
            return Ok();
        }
        #endregion

        [HttpGet("jobs")]
        [BrandRequired]
        public async Task<ActionResult<FilterListResponse<JobDTO>>> GetJobs([FromQuery] JobFilterDTO filter)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _jobService.GetAllJobByCurrentAccount(user, filter);
            return Ok(result);
        }

        /*[HttpPost("updatepremium")]
        [BrandRequired]
        public async Task UpdatePremiumBrand(UpdateVipRequestDTO updateVipRequest)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _paymentService.UpdateVipPaymentRequest(user.Id, updateVipRequest);
        }*/
    }
}
