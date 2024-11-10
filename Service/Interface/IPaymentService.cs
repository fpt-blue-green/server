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
        Task<PaymentCollectionLinkResponse> PaymentCollectionLink(CollectionLinkRequestDTO collectionLinkRequestDTO);
    }
}
