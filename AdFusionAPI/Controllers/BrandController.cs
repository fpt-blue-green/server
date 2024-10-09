using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly ICampaignService _campaignService;
        public BrandController(IBrandService brandService, ICampaignService campaignService)
        {
            _brandService = brandService;
            _campaignService = campaignService;
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
        public async Task<ActionResult<List<CampaignDTO>>> GetBrandCampaigns()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.GetBrandCampaigns(user.Id);
            return Ok(result);
        }
    }
}
