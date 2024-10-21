using AdFusionAPI.APIConfig;
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
        [HttpPut("{id}/reoffer")]
        public async Task<ActionResult> ReOffer(Guid id,ReOfferDTO reofferDto)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _offerService.ReOffer(id, user, reofferDto);
            return Ok();
        }

        [AuthRequired]
        [HttpPut("{id}/rejectOffer")]
        public async Task<ActionResult> RejectOffer(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _offerService.RejectOffer(id, user);
            return Ok();
        }

        [AuthRequired]
        [HttpPut("{id}/approveOffer")]
        public async Task<ActionResult> ApproveOffer(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _offerService.ApproveOffer(id, user);
            return Ok();
        }
    }
}
