using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task CreatePaymentHistory(PaymentHistory paymentHistory)
        {
            using (var context = new PostgresContext())
            {
                await context.PaymentHistories.AddAsync(paymentHistory);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PaymentHistory>> GetAll()
        {
            using (var context = new PostgresContext())
            {
                var paymentHistories = await context.PaymentHistories
                    .Include(i => i.User)
                    .ToListAsync();
                return paymentHistories;
            }
        }

        public async Task<PaymentHistory> GetPaymentHistoryById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                return await context.PaymentHistories.FirstOrDefaultAsync(p => p.Id == id);
            }
        }

        public async Task UpdatePaymentHistory(PaymentHistory paymentHistory)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(paymentHistory).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
