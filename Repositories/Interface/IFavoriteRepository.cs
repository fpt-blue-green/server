using BusinessObjects.Models;

namespace Repositories
{
    public interface IFavoriteRepository
    {
        Task CreateFavorite(Favorite favorite);
        Task DeleteFavorite(Guid favoriteId);
        Task DeleteFavoriteByInfluencerId(Guid influencerId);
        Task<IEnumerable<Favorite>> GetAllFavoriteByUserId(Guid userId);
    }
}
