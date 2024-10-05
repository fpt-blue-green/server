using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ICampaignContentService _campaignContentService;
        public CampaignsController(ICampaignService campaignService, ICampaignContentService campaignContentService)
        {
            _campaignService = campaignService;
            _campaignContentService = campaignContentService;
        }
        [HttpGet()]
        public async Task<ActionResult<List<CampaignBrandDto>>> GetCampaignsInprogres([FromQuery]  CampaignFilterDto filter)
        {
            var result = await _campaignService.GetCampaignsInprogres(filter);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignDTO>> GetCampaign(Guid id)
        {
            var result = await _campaignService.GetCampaign(id);
            return Ok(result);
        }

        [HttpPost("")]
        [BrandRequired]
        public async Task<ActionResult<Guid>> CreateCampaign([FromBody] CampaignDTO campaign)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.CreateCampaign(user.Id, campaign);
            return Ok(result);
        }

        [HttpPut]
        [BrandRequired]
        public async Task<ActionResult<Guid>> UpdateCampaign([FromBody] CampaignDTO campaign)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.UpdateCampaign(user.Id, campaign);
            return Ok(result);
        }
        /*[HttpGet("tags")]
		[AuthRequired]
		public async Task<ActionResult<List<TagDTO>>> GetTagsOfCampaign( Guid campaignId)
		{
			var result = await _campaignService.GetTagsOfCampaign(campaignId);
			return Ok(result);
		}*/

        [HttpPost("tags")]
        [BrandRequired]
        public async Task<ActionResult<string>> UpdateTagsOfCampaign(Guid campaignId, List<Guid> listTags)
        {
            var result = await _campaignService.UpdateTagsForCampaign(campaignId, listTags);
            return Ok(result);
        }

        [HttpPost("images")]
        [BrandRequired]
        public async Task<ActionResult<List<string>>> UploadImages([FromForm] List<Guid> imageIds, [FromForm] List<IFormFile> images, Guid campaginId)
        {
            var result = await _campaignService.UploadCampaignImages(campaginId, imageIds, images, "CampaignImages");
            return Ok(result);
        }

        #region content
        [HttpPost("contents")]
        [BrandRequired]
        public async Task<ActionResult> CreateCampaignContents([FromBody] List<CampaignContentDto> contents, Guid campaignId)
        {
            await _campaignContentService.CreateCampaignContents(campaignId, contents);
            return Ok();
        }
        /*[HttpPut("contents/{contentId}")]
		[AuthRequired]
		public async Task<ActionResult<string>> UpdateCampaignContent([FromBody] CampaignContentResDto content, Guid contentId,Guid campaignId)
		{
			var result = await _campaignContentService.UpdateCampaignContent(campaignId, contentId, content);
			return Ok(result);
		}*/
        [HttpGet("contents")]
        [BrandRequired]
        public async Task<ActionResult<List<InfluencerDTO>>> GetCampaignContents(Guid campaignId)
        {
            var result = await _campaignContentService.GetCampaignContents(campaignId);
            return Ok(result);
        }
        /*[HttpGet("contents/{contentId}")]
		[AuthRequired]
		public async Task<ActionResult<PackageDTO>> GetCampaignContent(Guid contentId)
		{
			var result = await _campaignContentService.GetCampaignContent(contentId);
			return Ok(result);
		}*/
        #endregion

    }
}
