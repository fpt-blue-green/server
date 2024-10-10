using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;


namespace Repositories.Implement
{
    public class AdminActionRepository : IAdminActionRepository
    {
        public async Task Create(AdminAction detail)
        {
            using (var context = new PostgresContext())
            {
                await context.AdminActions.AddAsync(detail);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AdminAction>> GetAdminActions()
        {
            using (var context = new PostgresContext())
            {
                return await context.AdminActions
                                        .OrderByDescending(a => a.ActionDate)
                                        .Include(a => a.User)
                                        .ToListAsync();
            }
        }

    }
}
