using BusinessObjects.Models;

namespace Repositories
{
    public interface IFavoriteRepository
    {
        Task CreateFavorite(Favorite favorite);
        Task DeleteFavorite(Guid favoriteId);
        Task DeleteFavoriteByInfluencerId(Favorite favorite);
        Task<IEnumerable<Favorite>> GetAllFavoriteByUserId(Guid userId);
    }
}
