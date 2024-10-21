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

        [InfluencerRequired]
        [HttpPut("{id}/link")]
        public async Task<ActionResult> AttachLink(Guid id, [FromBody] JobLinkDTO jobLinkDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.AttachPostLink(id, user, jobLinkDTO);
            return Ok();
        }
    }
}
