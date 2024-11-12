using BusinessObjects;
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

        public async Task<IEnumerable<PaymentHistory>> GetAllProfitPayment()
        {
            using (var context = new PostgresContext())
            {
                var paymentHistories = await context.PaymentHistories
                    .Where(i => i.Type == (int)EPaymentType.BuyPremium || i.Type == (int)EPaymentType.WithDraw)
                    .ToListAsync();
                return paymentHistories;
            }
        }

        public async Task<IEnumerable<PaymentHistory>> GetAllProfitPaymentIgnoreFilter()
        {
            using (var context = new PostgresContext())
            {
                var paymentHistories = await context.PaymentHistories
                    .Where(i => i.Type == (int)EPaymentType.BuyPremium || i.Type == (int)EPaymentType.WithDraw)
                    .IgnoreQueryFilters()
                    .ToListAsync();
                return paymentHistories;
            }
        }

        public async Task<PaymentHistory> GetPaymentHistoryById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var result =  await context.PaymentHistories
                                           .Include(p => p.User)
                                           .FirstOrDefaultAsync(p => p.Id == id);
                return result!;
            }
        }

        public async Task<List<PaymentHistory>> GetWithDrawPaymentHistoryByUserId(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.PaymentHistories
                                           .Include(p => p.User)
                                           .Where(p => p.UserId == id && p.Type == (int)EPaymentType.WithDraw && p.Status == (int)EPaymentStatus.Done)
                                           .ToListAsync();
                return result!;
            }
        }

        public async Task<PaymentHistory> GetPaymentHistoryPedingById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.PaymentHistories
                                           .Include(p => p.User)
                                           .FirstOrDefaultAsync(p => p.Id == id && p.Status == (int)EPaymentStatus.Pending);
                return result!;
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
