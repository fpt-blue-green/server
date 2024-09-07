using BusinessObjects.Models;
using BusinessObjects;

namespace Service
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> GetJWTSystemSetting();
        Task<SystemSetting> GetSystemSetting(string keyName);
        Task<string> UpdateSystemSetting(SystemSettingDTO systemSettingDTO);
    }
}
