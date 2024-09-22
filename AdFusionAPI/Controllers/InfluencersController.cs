using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfluencersController : Controller
    {
        private readonly IInfluencerService _influencerService;
        private readonly IChannelService _channelService;
        private readonly IFeedBackService _feedBackService;
        private readonly IMapper _mapper;

        public InfluencersController(IInfluencerService influencerService,IChannelService channelService, IMapper mapper, IFeedBackService feedBackService)
        {
            _influencerService = influencerService;
            _channelService = channelService;
            _mapper = mapper;
            _feedBackService = feedBackService;
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInfluencer()
        {
            var result = await _influencerService.GetTopInfluencer();
            return Ok(result);
        }

        [HttpGet("top/instagram")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInstagramInfluencer()
        {
            var result = await _influencerService.GetTopInstagramInfluencer();
            return Ok(result);
        }

        [HttpGet("top/tiktok")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopTiktokInfluencer()
        {
            var result = await _influencerService.GetTopTiktokInfluencer();
            return Ok(result);
        }

        [HttpGet("top/youtube")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopYoutubeInfluencer()
        {
            var result = await _influencerService.GetTopYoutubeInfluencer();
            return Ok(result);

        }

        [HttpGet]
        public async Task<ActionResult<GetInfluencersResponseDTO>> GetExploreInfluencer([FromQuery] InfluencerFilterDTO filterDTO)
        {
            var result = await _influencerService.GetAllInfluencers(filterDTO);
            return Ok(result);
        }


        [HttpGet("{slug}")]
        public async Task<ActionResult<InfluencerDTO>> GetInfluencerBySlug(string slug)
        {
            var result = await _influencerService.GetInfluencerBySlug(slug);
            return Ok(result);
        }

        #region Feedback
        [HttpGet("{id}/feedbacks")]
        public async Task<ActionResult<FeedbackDTO>> GetFeedbackByInfluencerId(Guid id)
        {
            var result = await _feedBackService.GetFeedBackByInfluencerId(id);
            return Ok(result);
        }

        [HttpPost("{id}/feedbacks")]
        [AuthRequired]
        public async Task<ActionResult> CreateFeedback(Guid id, [FromBody] FeedbackRequestDTO feedBackRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _feedBackService.CreateFeedback(id, feedBackRequestDTO, user);
            return Ok();
        }

        [HttpPut("{id}/feedbacks/{feedbackId}")]
        [AuthRequired]
        public async Task<ActionResult> UpdateFeedback(Guid id, Guid feedbackId, [FromBody] FeedbackRequestDTO feedBackRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _feedBackService.DeleteFeedback(id, feedbackId, user);
            return Ok();
        }

        [HttpDelete("{id}/feedbacks/{feedbackId}")]
        [AuthRequired]
        public async Task<ActionResult> DelteFeedback(Guid id, Guid feedbackId)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _feedBackService.DeleteFeedback(id, feedbackId, user);
            return Ok();
        }
        #endregion
    }
}