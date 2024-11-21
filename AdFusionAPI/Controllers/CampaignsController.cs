using AdFusionAPI.APIConfig;
using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;
using static BusinessObjects.JobEnumContainer;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ICampaignContentService _campaignContentService;
        private readonly IInfluencerService _influencerService;
        private readonly IJobDetailService _jobDetailService;
        private readonly ICampaignMeetingRoomService _campaignMeetingRoomService;
        public CampaignsController(ICampaignService campaignService, ICampaignContentService campaignContentService,
            IInfluencerService influencerService, IJobDetailService jobDetailService, ICampaignMeetingRoomService campaignMeetingRoomService)
        {
            _campaignService = campaignService;
            _campaignContentService = campaignContentService;
            _influencerService = influencerService;
            _jobDetailService = jobDetailService;
            _campaignMeetingRoomService = campaignMeetingRoomService;
        }
        [HttpGet()]
        public async Task<ActionResult<FilterListResponse<CampaignDTO>>> GetCampaignsInProgress([FromQuery] CampaignFilterDTO filter)
        {
            var result = await _campaignService.GetCampaignsInProgress(filter);
            return Ok(result);
        }

        [HttpGet("{id}/Influencers")]
        [BrandRequired]
        public async Task<ActionResult<FilterListResponse<InfluencerJobDTO>>> GetCampaignsJobInfluencer( Guid id ,[FromQuery] InfluencerJobFilterDTO filter)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _influencerService.GetInfluencerWithJobByCampaginId(id, filter, user);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignDTO>> GetCampaign(Guid id)
        {
            var result = await _campaignService.GetCampaign(id);
            return Ok(result);
        }

        [HttpGet("{id}/jobStatusCount")]
        [AuthRequired]
        public async Task<ActionResult<Dictionary<int, int>>> GetCampaignJobStatusData(Guid id)
        {
            var result = await _jobDetailService.GetCampaignJobStatus(id);
            return Ok(result);
        }

        [HttpGet("{id}/jobDetails")]
        [AuthRequired]
        public async Task<ActionResult<CampaignJobDetailDTO>> GetCampaignJobDetails(Guid id, [FromQuery]FilterDTO filter)
        {
            var result = await _jobDetailService.GetCampaignJobDetail(id, filter);
            return Ok(result);
        }

        [HttpGet("{id}/jobDetailBase")]
        [AuthRequired]
        public async Task<ActionResult<CampaignJobDetailBaseDTO>> GetCampaignJobDetailBase(Guid id)
        {
            var result = await _jobDetailService.GetCampaignJobDetailBaseData(id);
            return Ok(result);
        }

        [HttpGet("{id}/jobDetailBasePlatform")]
		[AuthRequired]
        public async Task<ActionResult<List<JobPlatFormPieChartDTO>>> GetCampaignJobDetailBasePlatform(Guid id)
        {
            var result = await _jobDetailService.GetCampaignJobDetailPlatForm(id);
            return Ok(result);
        }

        [HttpGet("{id}/jobDetailStatistic")]
        [AuthRequired]
        public async Task<ActionResult<List<CampaignDailyStatsDTO>>> GetCampaignJobDetailStatistic(Guid id)
        {
            var result = await _jobDetailService.GetCampaignDailyStats(id);
            return Ok(result);
        }

        [HttpGet("{id}/meetingRoom")]
        [AuthRequired]
        public async Task<ActionResult<IEnumerable<CampaignMeetingRoomDTO>>> GetCampaignMeetingRoom(Guid id)
        {
            var result = await _campaignMeetingRoomService.GetMeetingRooms(id);
            return Ok(result);
        }

        [HttpGet("{id}/participant")]
        [BrandRequired]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetCampaignParticipantInfluencer(Guid id)
        {
            var result = await _campaignService.GetCampaignParticipantInfluencer(id);
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
        public async Task<ActionResult> StartCampaign(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _campaignService.StartCampaign(id, user);
            return Ok();
        }

        [HttpPut("{id}/end")]
        [BrandRequired]
        public async Task<ActionResult> EndCampaign(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _campaignService.EndCampaign(id, user);
            return Ok();
        }

        [HttpPut("{id}/publish")]
        [BrandRequired]
        public async Task<ActionResult> PublishCampaign(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _campaignService.PublishCampaign(id, user);
            return Ok();
        }

        [HttpDelete("{id}")]
        [BrandRequired]
        public async Task<ActionResult> DeleteCampaign(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _campaignService.DeleteCampaign(id, user);
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
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _campaignService.UpdateTagsForCampaign(id, listTags, user);
            return Ok();
        }
        #endregion

        #region image
        [HttpPost("{id}/images")]
        [BrandRequired]
        public async Task<ActionResult<List<string>>> UploadImages([FromForm] List<Guid> imageIds, [FromForm] List<IFormFile> images, Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignService.UploadCampaignImages(id, imageIds, images, "CampaignImages", user);
            return Ok(result);
        }
        #endregion

        #region content
        [HttpPost("{id}/contents")]
        [BrandRequired]
        public async Task<ActionResult> CreateCampaignContents([FromBody] List<CampaignContentDto> contents, Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _campaignContentService.CreateCampaignContents(id, contents, user);
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
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _campaignContentService.GetCampaignContents(id, user);
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
