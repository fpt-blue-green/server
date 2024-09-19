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
    public class FeedbackController : Controller
    {
        private readonly IFeedBackService _feedBackService;
        private readonly IMapper _mapper;

        public FeedbackController(IMapper mapper, IFeedBackService feedBackService)
        {
            _mapper = mapper;
            _feedBackService = feedBackService;
        }

        [HttpPut]
        [AuthRequired]
        public async Task<ActionResult<string>> CreateFeedback([FromBody] FeedbackRequestDTO feedBackRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _feedBackService.CreateFeedback(feedBackRequestDTO, user);
            return Ok();
        }

        [HttpDelete]
        [AuthRequired]
        public async Task<ActionResult<string>> DelteFeedback([FromBody] FeedbackDeleteRequestDTO feedbackDeleteRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _feedBackService.DeleteFeedback(feedbackDeleteRequestDTO.Id, user);
            return Ok();
        }
    }
}
