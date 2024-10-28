using BusinessObjects.Models;

namespace Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public async Task CreatePaymentBooking(PaymentBooking paymentBooking)
        {
            using (var context = new PostgresContext())
            {
                await context.PaymentBookings.AddAsync(paymentBooking);
                await context.SaveChangesAsync();
            }
        }
    }
}
