using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class BannedUserRepository : IBannedUserRepository
    {
        public async Task CreateBannedUserData(BannedUser user)
        {
            using (var context = new PostgresContext())
            {
                context.BannedUsers.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateBannedUserData(BannedUser user)
        {
            using (var context = new PostgresContext())
            {
                context.Entry<BannedUser>(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BannedUser>> GetBannedUsers()
        {
            using (var context = new PostgresContext())
            {
                var result = await context.BannedUsers.Include(i => i.BannedBy).Include(i => i.User).ToListAsync();
                return result;
            }
        }

    }
}
