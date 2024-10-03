using BusinessObjects.Models;

namespace Repositories
{
    public interface IOfferRepository
    {
        Task Create(Offer offer);
    }
}
