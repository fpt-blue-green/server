using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly ICampaignService _campaignService;

        public BrandsController(IBrandService brandService, ICampaignService campaignService)
        {
            _brandService = brandService;
            _campaignService = campaignService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDTO>> GetBrandById(Guid id)
        {
            var result = await _brandService.GetBrandById(id);
            return Ok(result);
        }

        [HttpGet("{id}/campaigns")]
        public async Task<ActionResult<List<CampaignDTO>>> GetBrandCampaigns(Guid id)
        {
            var result = await _campaignService.GetAvailableBrandCampaigns(id);
            return Ok(result);
        }

    }
}
