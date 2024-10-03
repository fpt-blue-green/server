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
        [HttpPost("payment/{id}")]
        public async Task<ActionResult> BrandPayment(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _jobService.BrandPaymentJob(id ,user);
            return Ok();
        }

        [BrandRequired]
        [HttpPost("cancel/{id}")]
        public async Task<ActionResult> BrandCancel(Guid id)
        {
            await _jobService.BrandCancleJob(id);
            return Ok();
        }
    }
}
