using BusinessObjects.Models;

namespace Repositories
{
    public interface IPaymentRepository
    {
        Task CreatePaymentBooking(PaymentBooking paymentBooking);
        Task CreatePaymentHistory(PaymentHistory paymentHistory);
        Task<PaymentHistory> GetPaymentHistoryById(Guid id);
        Task UpdatePaymentHistory(PaymentHistory paymentHistory);
        Task<IEnumerable<PaymentHistory>> GetAll();
        Task<PaymentHistory> GetPaymentHistoryPedingById(Guid id);
    }
}
