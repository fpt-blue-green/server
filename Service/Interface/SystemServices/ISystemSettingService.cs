using BusinessObjects.Models;
using BusinessObjects;

namespace Service
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> GetJWTSystemSetting();
        Task<SystemSettingDTO> GetSystemSetting(string keyName);
        Task UpdateSystemSetting(SystemSettingDTO systemSettingDTO, UserDTO user);
    }
}
