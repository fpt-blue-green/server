using BusinessObjects.Models;

namespace Repositories
{
    public interface IBannedUserRepository
    {
        Task CreateBannedUserData(BannedUser user);
    }
}
