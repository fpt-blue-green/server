using BusinessObjects;
using BusinessObjects.Models;

namespace Repositories.Interface
{
    public interface IUserDeviceRepository
    {
        Task<UserDevice> GetUserDeviceByAgentAndToken(BrowserInfo userAgentConverted, string refreshToken);
        Task<UserDevice> GetUserDeviceByAgentAndUserID(BrowserInfo userAgentConverted, Guid userId);
        Task<IEnumerable<UserDevice>> GetByUserId(Guid userId);
        Task Update(UserDevice userDevice);
        Task Create(UserDevice userDevice);
    }
}
