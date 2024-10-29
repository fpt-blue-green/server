using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("withdraw")]
        [AuthRequired]
        public async Task<ActionResult> WithDraw(WithdrawRequestDTO withdrawRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _paymentService.CreatePaymentWithDraw(user, withdrawRequestDTO);
            return Ok();
        }

        [HttpPost("responseWithdraw/{id}")]
        [AdminRequired]
        public async Task<ActionResult> ResponseWithDraw(AdminPaymentResponse adminPaymentResponse, Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _paymentService.ProcessWithdrawalApproval(adminPaymentResponse, id, user);
            return Ok();
        }

        [HttpGet]
        [AdminRequired]
        public async Task<ActionResult<FilterListResponse<PaymentHistoryDTO>>> GetExplorePaymentHistory([FromQuery] PaymentWithDrawFilterDTO filter)
        {

            var result = await _paymentService.GetAllPayment(filter);
            return Ok(result);
        }
    }
}
