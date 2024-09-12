using BusinessObjects.Models;

namespace Repositories
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting> GetSystemSetting(string keyName);
        Task UpdateSystemSettingKeyValue(SystemSetting systemSetting);
    }
}
