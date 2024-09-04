using BusinessObjects.Enum;
using BusinessObjects.Models;
using BusinessObjects.DTOs;
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

        public async Task<SystemSetting> GetSystemSetting(string keyName)
        {
            var data = await _systemSettingRepository.GetSystemSetting(keyName);
            return data;

        }

        public async Task<string> UpdateSystemSetting(SystemSettingDTO systemSettingDTO)
        {
            var systemSetting = await _systemSettingRepository.GetSystemSetting(systemSettingDTO.KeyName);
            if (systemSetting == null)
            {
                throw new KeyNotFoundException();
            }
            systemSetting.KeyValue = systemSettingDTO.KeyValue;
            await _systemSettingRepository.UpdateSystemSetingKeyValue(systemSetting);
            return "Cập nhập cài đặt hệ thông thành công.";
        }
    }
}
