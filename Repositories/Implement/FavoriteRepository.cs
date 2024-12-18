﻿using BusinessObjects.Models;
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

        public async Task DeleteFavoriteByInfluencerId(Favorite favorite)
        {
            using (var context = new PostgresContext())
            {
                context.Favorites.Remove(favorite!);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Favorite>> GetAllFavoriteByUserId(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var favorites = await context.Favorites
                    .Where(f => f.Brand.UserId == userId)
                    .Include(f => f.Influencer)
                        .ThenInclude(i => i.User)
                    .Include(f => f.Influencer)
                        .ThenInclude(i => i.Channels)
                    .Include(f => f.Influencer)
                        .ThenInclude(i => i.InfluencerImages)
                    .ToListAsync();
                return favorites;
            }
        }
    }
}
