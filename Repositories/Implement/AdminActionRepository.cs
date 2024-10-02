using BusinessObjects.Models;


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
    }
}
