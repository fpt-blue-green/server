﻿using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : Controller
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [AuthRequired]
        [HttpPost]
        public async Task<ActionResult> CreateOffer(OfferCreateRequestDTO offerCreateRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _offerService.CreateOffer(user, offerCreateRequestDTO);
            return Ok();
        }
        [AuthRequired]
        [HttpPut("reoffer/{id}")]
        public async Task<ActionResult> ReOffer(Guid id,ReOfferDTO reofferDto)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _offerService.ReOffer(id, user, reofferDto);
            return Ok();
        }
    }
}