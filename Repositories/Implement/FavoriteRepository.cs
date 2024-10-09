using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        public async Task CreateFavorite(Favorite favorite)
        {
            using (var context = new PostgresContext())
            {
                await context.Favorites.AddAsync(favorite);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteFavorite(Guid favoriteId)
        {
            using (var context = new PostgresContext())
            {
                var favorite = await context.Favorites.FirstOrDefaultAsync(i => i.Id == favoriteId);
                context.Favorites.Remove(favorite!);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Favorite>> GetAllFavoriteByBrandId(Guid brandId)
        {
            using (var context = new PostgresContext())
            {
                var favorites = await context.Favorites.Where(i => i.BrandId == brandId).ToListAsync();
                return favorites;
            }
        }
    }
}
