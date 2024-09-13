﻿using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsers()
        {
            using (var context = new PostgresContext())
            {
                var users = await context.Users.ToListAsync();
                return users;
            }
        }

        public async Task<User> GetUserById(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.BannedUserUsers)
                    .Include(u => u.Influencer)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
                return user!;
            }
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.BannedUserUsers)
                    .Include(u => u.Influencer)
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsDeleted == false);
                return user!;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.Influencer)
                    .Include(u => u.BannedUserUsers)
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsDeleted == false);
                return user!;
            }
        }

        public async Task<User> GetUserByLoginDTO(LoginDTO loginDTO)
        {
            using (var context = new PostgresContext())
            {
                var user = await context.Users
                    .Include(u => u.BannedUserUsers)
                    .Include(u => u.Influencer)
                    .Where(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password && u.IsDeleted == false)
                    .FirstOrDefaultAsync();
                return user!;
            }
        }

        public async Task CreateUser(User user)
        {
            using (var context = new PostgresContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User user)
        {
            using (var context = new PostgresContext())
            {
                var localUser = context.Set<User>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(user.Id));

                if (localUser != null)
                {
                    context.Entry(localUser).State = EntityState.Detached;
                }

                context.Entry(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
