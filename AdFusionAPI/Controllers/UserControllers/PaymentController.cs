using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.UserControllers
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
        public async Task<ActionResult> ResponseWithDraw(Guid paymentId, AdminPaymentResponse adminPaymentResponse)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _paymentService.ProcessWithdrawalApproval(paymentId, adminPaymentResponse, user);
            return Ok();
        }

        [HttpGet]
        [AdminRequired]
        public async Task<ActionResult<FilterListResponse<PaymentHistoryDTO>>> GetExplorePaymentHistory([FromQuery] PaymentWithDrawFilterDTO filter)
        {
            var result = await _paymentService.GetAllPayment(filter);
            return Ok(result);
        }

        [HttpPost("vietQR/requestId")]
        [AdminRequired]
        public async Task<ActionResult<string>> GetVietQr(WithdrawRequestDTO withdrawRequestDTO, Guid requestId)
        {
            var result = await _paymentService.CreateWithDrawQR(withdrawRequestDTO, requestId);
            return Ok(result);
        }

        /* [HttpPost("responseUpdatePremium/{paymentId}")]
         [AdminRequired]
         public async Task<ActionResult> ResponseUpdatePremium(Guid paymentId, AdminPaymentResponse adminPaymentResponse)
         {
             var user = (UserDTO)HttpContext.Items["user"]!;
             await _paymentService.ProcessUpdatePremiumApproval(paymentId, adminPaymentResponse, user);
             return Ok();
         }*/

        [HttpPost("updatePremium/createCollectionLink")]
        [BrandRequired]
        public async Task<ActionResult<PaymentCollectionLinkResponse>> UpdatePremiumCollectionLink(UpdatePremiumRequestDTO updatePremiumRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _paymentService.UpdatePremium(updatePremiumRequestDTO, user);
            return Ok(result);
        }

        [HttpPost("updatePremium/callback")]
        public async Task<IActionResult> UpdatePremiumCallback([FromBody] CallbackDTO callbackDTO)
        {
            await _paymentService.UpdatePremiumCallBack(callbackDTO);
            return Ok();
        }

        [HttpPost("deposit/createCollectionLink")]
        [BrandRequired]
        public async Task<ActionResult<PaymentCollectionLinkResponse>> DepositCollectionLink(DepositRequestDTO depositRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _paymentService.Deposit(depositRequestDTO, user);
            return Ok(result);
        }

        [HttpPost("deposit/callback")]
        public async Task<IActionResult> DepositCallback([FromBody] CallbackDTO callbackDTO)
        {
            await _paymentService.DepositCallBack(callbackDTO);
            return Ok();
        }
    }
}
