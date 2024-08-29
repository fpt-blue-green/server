using BusinessObjects.Models;
using BusinessObjects.DTOs.AuthDTO;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class UserRepository : SingletonBase<UserRepository>, IUserRepository
    {
        public UserRepository() { }
        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                var users = await context.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserById(Guid userId)
        {
            try
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
                return user!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            try
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsDeleted == false);
                return user!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email && u.IsDeleted == false);
                    return user!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByLoginDTO(LoginDTO loginDTO)
        {
            try
            {
                var user = await context.Users
                                    .Include(u => u.BannedUserUsers).Include(u => u.Influencers)
                                    .Where(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password && u.IsDeleted == false)
                                    .FirstOrDefaultAsync();
                return user!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateUser(User user)
        {
            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
