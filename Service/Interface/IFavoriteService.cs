using BusinessObjects;

namespace Service
{
    public interface IFavoriteService
    {
        Task CreateFavorite(Guid id, UserDTO user);
        Task DeleteFavoriteById(Guid favoriteId);
        Task DeleteFavoriteByInfluencerId(Guid influencerId);
        Task<IEnumerable<FavoriteDTO>> GetAllFavorites(UserDTO user);
    }
}
