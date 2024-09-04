using BusinessObjects.Models;
using BusinessObjects.DTOs;

namespace Service.Interface.SystemServices
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> GetJWTSystemSetting();
        Task<SystemSetting> GetSystemSetting(string keyName);
        Task<string> UpdateSystemSetting(SystemSettingDTO systemSettingDTO);
    }
}
