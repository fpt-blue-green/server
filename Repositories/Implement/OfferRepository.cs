using BusinessObjects.Models;

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
    }
}
