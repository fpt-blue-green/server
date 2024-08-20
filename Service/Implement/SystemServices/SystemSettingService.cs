using BusinessObjects.Enum;
using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using Repositories.Implement;
using Repositories.Interface;
using Service.Domain;
using Service.Interface.SystemServices;

namespace Service.Implement.SystemService
{

    public class SystemSettingService : ISystemSettingService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();

        public async Task<SystemSetting> GetJWTSystemSetting()
        {
            return await _systemSettingRepository.GetSystemSetting(_configManager.JWTKey);
        }

        public async Task<ApiResponse<SystemSetting>> GetSystemSetting(string keyName)
        {
            var data = await _systemSettingRepository.GetSystemSetting(keyName);
            return new ApiResponse<SystemSetting>
            {
                StatusCode = data != null ? EHttpStatusCode.OK : EHttpStatusCode.NotFound,
                Message = data != null ? "Truy vấn thông tin cài đặt hệ thống thành công." : "KeyName không tồn tại trong hệ thống.",
                Data = data
            };

        }

        public async Task<ApiResponse<string>> UpdateSystemSetting(SystemSettingDTO systemSettingDTO)
        {
            try
            {
                var systemSetting = await _systemSettingRepository.GetSystemSetting(systemSettingDTO.KeyName);
                if (systemSetting == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.NotFound,
                        Message = "KeyName không tồn tại trong hệ thống.",
                        Data = null
                    };
                }
                systemSetting.KeyValue = systemSettingDTO.KeyValue;
                await _systemSettingRepository.UpdateSystemSetingKeyValue(systemSetting);
                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Cập nhập cài đặt hệ thông thành công.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }
    }
}
