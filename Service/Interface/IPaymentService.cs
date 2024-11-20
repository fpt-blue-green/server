using BusinessObjects;

namespace Service
{
    public interface IPaymentService
    {
        Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO);
        Task ProcessWithdrawalApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO user);
        Task<FilterListResponse<PaymentHistoryDTO>> GetAllPayment(PaymentWithDrawFilterDTO filter);
        Task UpdateVipPaymentRequest(string requestDto);
        Task ProcessUpdatePremiumApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO user);
        Task<PaymentCollectionLinkResponse> UpdatePremium(UpdatePremiumRequestDTO updatePremiumRequestDTO, UserDTO userDto);
        Task UpdatePremiumCallBack(CallbackDTO callbackDTO);
        Task<PaymentCollectionLinkResponse> Deposit(DepositRequestDTO depositRequestDTO, UserDTO userDto);
        Task DepositCallBack(CallbackDTO callbackDTO);
        Task<string> CreateWithDrawQR(WithdrawRequestDTO withdrawRequestDTO, Guid requestId);
    }
}
