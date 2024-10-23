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
        public async Task<ActionResult<List<CampaignDTO>>> GetCampaignsInprogres([FromQuery] CampaignFilterDto filter)
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
        public async Task<ActionResult<Guid>> CreateCampaign([FromBody] CampaignResDto campaign)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.CreateCampaign(user.Id, campaign);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [BrandRequired]
        public async Task<ActionResult<Guid>> UpdateCampaign([FromBody] CampaignResDto campaign, Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.UpdateCampaign(user.Id, id, campaign);
            return Ok(result);
        }
        [HttpPut("{id}/start")]
        [BrandRequired]
        public async Task<ActionResult<Guid>> StartCampaign(Guid id)
        {
            await _campaignService.StartCampaign(id);
            return Ok();
        }
        [HttpPut("{id}/publish")]
        [BrandRequired]
        public async Task<ActionResult<Guid>> PublishCampaign(Guid id)
        {
            await _campaignService.PublishCampaign(id);
            return Ok();
        }
        #region tag
        /*[HttpGet("tags")]
		[AuthRequired]
		public async Task<ActionResult<List<TagDTO>>> GetTagsOfCampaign( Guid campaignId)
		{
			var result = await _campaignService.GetTagsOfCampaign(campaignId);
			return Ok(result);
		}*/

        [HttpPost("{id}/tags")]
        [BrandRequired]
        public async Task<ActionResult> UpdateTagsOfCampaign(Guid id, List<Guid> listTags)
        {
            await _campaignService.UpdateTagsForCampaign(id, listTags);
            return Ok();
        }
        #endregion
        #region image
        [HttpPost("{id}/images")]
        [BrandRequired]
        public async Task<ActionResult<List<string>>> UploadImages([FromForm] List<Guid> imageIds, [FromForm] List<IFormFile> images, Guid id)
        {
            var result = await _campaignService.UploadCampaignImages(id, imageIds, images, "CampaignImages");
            return Ok(result);
        }
        #endregion

        #region content
        [HttpPost("{id}/contents")]
        [BrandRequired]
        public async Task<ActionResult> CreateCampaignContents([FromBody] List<CampaignContentDto> contents, Guid id)
        {
            await _campaignContentService.CreateCampaignContents(id, contents);
            return Ok();
        }
        /*[HttpPut("contents/{contentId}")]
		[AuthRequired]
		public async Task<ActionResult<string>> UpdateCampaignContent([FromBody] CampaignContentResDto content, Guid contentId,Guid campaignId)
		{
			var result = await _campaignContentService.UpdateCampaignContent(campaignId, contentId, content);
			return Ok(result);
		}*/
        [HttpGet("{id}/contents")]
        [BrandRequired]
        public async Task<ActionResult<List<CampaignContentDto>>> GetCampaignContents(Guid id)
        {
            var result = await _campaignContentService.GetCampaignContents(id);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [BrandRequired]
        public async Task<ActionResult> DeleteCampaign(Guid id)
        {
            await _campaignService.DeleteCampaign(id);
            return Ok();
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
