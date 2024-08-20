using BusinessObjects.Models;
using BusinessObjects.DTOs;

namespace Service.Interface.SystemServices
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> GetJWTSystemSetting();
        Task<ApiResponse<SystemSetting>> GetSystemSetting(string keyName);
        Task<ApiResponse<string>> UpdateSystemSetting(SystemSettingDTO systemSettingDTO);
    }
}
