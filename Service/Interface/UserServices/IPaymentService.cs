using BusinessObjects;

namespace Service
{
    public interface IPaymentService
    {
        Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO);
        Task<bool> ProcessWithdrawalApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO user);
        Task<FilterListResponse<PaymentHistoryDTO>> GetAllPayment(PaymentWithDrawFilterDTO filter);
        Task UpdateVipPaymentRequest(string requestDto);
        Task<PaymentCollectionLinkResponse> UpdatePremium(UpdatePremiumRequestDTO updatePremiumRequestDTO, UserDTO userDto);
        Task UpdatePremiumCallBack(CallbackDTO callbackDTO);
        Task<PaymentCollectionLinkResponse> Deposit(DepositRequestDTO depositRequestDTO, UserDTO userDto);
        Task DepositCallBack(CallbackDTO callbackDTO);
        Task<string> CreateWithDrawQR(Guid requestId);
    }
}
