using BusinessObjects;

namespace Service
{
    public interface IPaymentService
    {
        Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO);
        Task ProcessWithdrawalApproval(AdminPaymentResponse adminPaymentResponse, Guid id, UserDTO user);
        Task<FilterListResponse<PaymentHistoryDTO>> GetAllPayment(PaymentWithDrawFilterDTO filter);
    }
}
