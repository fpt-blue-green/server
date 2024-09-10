using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserRepository : SingletonBase<UserRepository>, IUserRepository
    {
        public UserRepository() { }
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var user = await context.Users
                                    .Include(u => u.BannedUserUsers).Include(u => u.Influencer)
                                    .SingleOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            return user!;
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var user = await context.Users
                .Include(u => u.BannedUserUsers).Include(u => u.Influencer)
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsDeleted == false);
            return user!;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await context.Users.Include(u => u.Influencer)
                                        .Include(u => u.BannedUserUsers).Include(u => u.Influencer)
                                        .SingleOrDefaultAsync(u => u.Email == email && u.IsDeleted == false);
            return user!;
        }

        public async Task<User> GetUserByLoginDTO(LoginDTO loginDTO)
        {
            var user = await context.Users
                                    .Include(u => u.BannedUserUsers).Include(u => u.Influencer)
                                    .Where(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password && u.IsDeleted == false)
                                    .FirstOrDefaultAsync();
            return user!;
        }

        public async Task CreateUser(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        public async Task UpdateUser(User user)
        {
            user.ModifiedAt = DateTime.UtcNow;
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
