using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IJobDetailService _jobDetailService;
        private readonly IOfferService _offerService;

        public JobController(IJobService jobService, IOfferService offerService, IJobDetailService jobDetailService)
        {
            _jobService = jobService;
            _offerService = offerService;
            _jobDetailService = jobDetailService;
        }

        [BrandRequired]
        [HttpPut("{id}/payment")]
        public async Task<ActionResult> BrandPayment(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandPaymentJob(id, user);
            return Ok();
        }

        [BrandRequired]
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> BrandCancel(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandCancelJob(id, user);
            return Ok();
        }

        [BrandRequired]
        [HttpPut("{id}/complete")]
        public async Task<ActionResult> BrandComplete(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandCompleteJob(id, user);
            return Ok();
        }

        [BrandRequired]
        [HttpPut("{id}/fail")]
        public async Task<ActionResult> BrandFail(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandFaliedJob(id, user);
            return Ok();
        }

        [BrandRequired]
        [HttpPut("{id}/reopen")]
        public async Task<ActionResult> BrandReopen(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandReopenJob(id, user);
            return Ok();
        }

        [InfluencerRequired]
        [HttpPut("{id}/link")]
        public async Task<ActionResult> AttachLink(Guid id, [FromBody] JobLinkDTO jobLinkDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.AttachPostLink(id, user, jobLinkDTO);
            return Ok();
        }

        [BrandRequired]
        [HttpPut("{id}/approveLink")]
        public async Task<ActionResult> ApprovePostLink(Guid id, [FromBody] String link)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.ApprovePostLink(id, link);
            return Ok();
        }

        [AuthRequired]
        [HttpGet("{id}/link")]
        public async Task<ActionResult> GetLink(Guid id)
        {
            var result =  await _jobService.GetJobLink(id);
            return Ok(result);
        }

        [AuthRequired]
        [HttpGet("{id}/offer")]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetOfferByJobId(Guid id)
        {
            var result = await _offerService.GetOfferByJobId(id);
            return Ok(result);
        }

        [AuthRequired]
        [HttpGet("statistical")]
        public async Task<ActionResult<List<JobStatistical>>> JobStatistical()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _jobService.Statistical(user);
            return Ok(result);
        }

        [AuthRequired]
        [HttpGet("{jobId}/JobDetails")]
        public async Task<ActionResult<List<CampaignDailyStatsDTO>>> JobDailyStatistical(Guid jobId, [FromQuery] string? link = null)
        {
            var result = await _jobDetailService.GetJobDailyStats(jobId, link);
            return Ok(result);
        }

    }
}
