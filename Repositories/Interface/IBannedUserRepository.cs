using BusinessObjects.Models;

namespace Repositories
{
    public interface IBannedUserRepository
    {
        Task CreateBannedUserData(BannedUser user);
        Task UpdateBannedUserData(BannedUser user);
        Task<IEnumerable<BannedUser>> GetBannedUsers();
    }
}
