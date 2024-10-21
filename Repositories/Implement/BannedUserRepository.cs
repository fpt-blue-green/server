using BusinessObjects.Models;

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

    }
}
