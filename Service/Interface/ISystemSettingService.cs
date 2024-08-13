using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;

namespace Service.Interface
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> GetJWTSystemSetting();
        Task<ApiResponse<SystemSetting>> GetSystemSetting(string keyName);
        Task<ApiResponse<string>> UpdateSystemSetting(SystemSettingDTO systemSettingDTO);
    }
}
