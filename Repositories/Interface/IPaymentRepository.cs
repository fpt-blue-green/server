using BusinessObjects.Models;

namespace Repositories
{
    public interface IPaymentRepository
    {
        Task CreatePaymentBooking(PaymentBooking paymentBooking);
    }
}
