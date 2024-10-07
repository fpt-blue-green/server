using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repositories
{
    public class OfferRepository : IOfferRepository
    {
        public async Task Create(Offer offer)
        {
            using (var context = new PostgresContext())
            {
                await context.Offers.AddAsync(offer);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Offer> GetById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                return (await context.Offers.Include(o => o.Job).FirstOrDefaultAsync(s => s.Id == id))!;
            }
        }

        public async Task Update(Offer offer)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(offer).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateJobAndOffer(Offer offer)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(offer).State = EntityState.Modified;

                // Cập nhật trạng thái của job con trong Offer
                if (offer.Job != null)
                {
                    context.Entry(offer.Job).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
