using BusinessObjects.Models;

namespace Repositories
{
    public interface IPaymentBookingRepository
    {
        Task Create(PaymentBooking paymentBooking);
    }
}
