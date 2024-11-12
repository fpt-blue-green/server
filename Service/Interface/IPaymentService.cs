using BusinessObjects;

namespace Service
{
    public interface IPaymentService
    {
        Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO);
        Task ProcessWithdrawalApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO user);
        Task<FilterListResponse<PaymentHistoryDTO>> GetAllPayment(PaymentWithDrawFilterDTO filter);
        Task UpdateVipPaymentRequest(Guid userId, UpdateVipRequestDTO updateVipRequest);
        Task ProcessUpdatePremiumApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO user);
        Task<PaymentCollectionLinkResponse> UpdatePremium(UpdatePremiumRequestDTO updatePremiumRequestDTO, UserDTO userDto);
        Task UpdatePremiumCallBack(CallbackDTO callbackDTO);
        Task<PaymentCollectionLinkResponse> Deposit(DepositRequestDTO depositRequestDTO, UserDTO userDto);
        Task DepositCallBack(CallbackDTO callbackDTO);
    }
}
