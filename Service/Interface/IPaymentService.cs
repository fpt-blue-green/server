using BusinessObjects;

namespace Service
{
    public interface IPaymentService
    {
        Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO);
        Task ResponseWithDraw(AdminPaymentResponse adminPaymentResponse, Guid id);
        Task<PaymentResponseDTO> GeAllPayment(PaymentWithDrawFilterDTO filter);
    }
}
