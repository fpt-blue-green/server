using BusinessObjects;

namespace Service
{
    public interface IFavoriteService
    {
        Task CreateFavorite(Guid id, UserDTO user);
        Task DeleteFavorite(Guid favoriteId);
        Task<IEnumerable<FavoriteDTO>> GetAllFavorites(UserDTO user);
    }
}
