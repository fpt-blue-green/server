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

        public async Task<PaymentBooking> GetInfluencerPaymentByJobId(Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.PaymentBookings
                    .FirstOrDefaultAsync(j => j.JobId == jobId && j.Type == (int)EPaymentType.InfluencerPayment);
                return result;
            }
        }

        public async Task<PaymentBooking> GetBrandPaymentByJobId(Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.PaymentBookings
                    .FirstOrDefaultAsync(j => j.JobId == jobId && j.Type == (int)EPaymentType.BrandPayment);
                return result;
            }
        }

        public async Task<IEnumerable<PaymentHistory>> GetAllPaymentsHistory()
        {
            using (var context = new PostgresContext())
            {
                // First query: Get payment histories
                var userPaymentHistories = await context.PaymentHistories
                                                .Include(i => i.User)
                                                .ToListAsync();

                // Second query: Get payment bookings
                var paymentHistories = await context.PaymentBookings
                                                .Include(i => i.Job)
                                                    .ThenInclude(j => j.Influencer)
                                                        .ThenInclude(i => i.User)
                                                .Select(pb => new PaymentHistory
                                                {
                                                    Id = pb.Id, 
                                                    UserId = pb.Job!.Influencer.User.Id,
                                                    Amount = pb.Amount ?? 0,
                                                    NetAmount = 0,
                                                    BankInformation = null!,
                                                    Status = (int)EPaymentStatus.Done,
                                                    Type = (int)pb.Type!,
                                                    CreatedAt = pb.PaymentDate!.Value,
                                                    ResponseAt = pb.PaymentDate!.Value,
                                                    AdminMessage = null,
                                                    User = pb.Job.Influencer.User 
                                                })
                                                .ToListAsync();



                // Concatenate the three results
                var combinedUserPayments = userPaymentHistories.Concat(paymentHistories);

                return combinedUserPayments;
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
                    .Where(i => (i.Type == (int)EPaymentType.BuyPremium || i.Type == (int)EPaymentType.WithDraw) && i.Status == (int)EPaymentStatus.Done)
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
