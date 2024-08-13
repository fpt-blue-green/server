using BusinessObjects.Models;

namespace Repositories.Interface
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting> GetSystemSetting(string keyName);
        Task UpdateSystemSetingKeyValue(SystemSetting systemSetting);
    }
}
