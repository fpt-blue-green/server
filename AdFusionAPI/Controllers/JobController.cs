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

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [BrandRequired]
        [HttpPut("payment/{id}")]
        public async Task<ActionResult> BrandPayment(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandPaymentJob(id, user);
            return Ok();
        }

        [BrandRequired]
        [HttpPut("cancel/{id}")]
        public async Task<ActionResult> BrandCancel(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandCancelJob(id, user);
            return Ok();
        }

        [InfluencerRequired]
        [HttpPut("link/{id}")]
        public async Task<ActionResult> AttachLink(Guid id, [FromBody] JobLinkDTO jobLinkDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.AttachPostLink(id, user, jobLinkDTO);
            return Ok();
        }
    }
}
