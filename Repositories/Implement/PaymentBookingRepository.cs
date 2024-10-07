using BusinessObjects.Models;

namespace Repositories
{
    public class PaymentBookingRepository : IPaymentBookingRepository
    {
        public async Task Create(PaymentBooking paymentBooking)
        {
            using (var context = new PostgresContext())
            {
                await context.PaymentBookings.AddAsync(paymentBooking);
                await context.SaveChangesAsync();
            }
        }
    }
}
