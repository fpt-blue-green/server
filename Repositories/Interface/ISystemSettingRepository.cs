using BusinessObjects.Models;

namespace Repositories
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting> GetSystemSetting(string keyName);
        Task UpdateSystemSetingKeyValue(SystemSetting systemSetting);
    }
}
