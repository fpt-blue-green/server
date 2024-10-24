﻿using AdFusionAPI.APIConfig;
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
        private readonly IOfferService _offerService;

        public JobController(IJobService jobService, IOfferService offerService)
        {
            _jobService = jobService;
            _offerService = offerService;
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
    }
}
