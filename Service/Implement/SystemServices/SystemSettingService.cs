using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Service.Helper;
using Supabase.Gotrue;

namespace Service
{

    public class SystemSettingService : ISystemSettingService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();
        private static AdminActionNotificationHelper adminActionNotificationHelper = new AdminActionNotificationHelper();

        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task<SystemSetting> GetJWTSystemSetting()
        {
            return await _systemSettingRepository.GetSystemSetting(_configManager.JWTKey);
        }

        public async Task<SystemSettingDTO> GetSystemSetting(string keyName)
        {
            var data = await _systemSettingRepository.GetSystemSetting(keyName);
            var result = _mapper.Map<SystemSettingDTO>(data);
            if (result == null)
            {
                throw new KeyNotFoundException();
            }
            return result;
        }

        public async Task<string> UpdateSystemSetting(SystemSettingDTO systemSettingDTO, UserDTO user)
        {
            var systemSetting = await _systemSettingRepository.GetSystemSetting(systemSettingDTO.KeyName);
            var oldData = systemSetting.KeyValue;
            if (systemSetting == null)
            {
                throw new KeyNotFoundException();
            }
            systemSetting.KeyValue = systemSettingDTO.KeyValue;
            await _systemSettingRepository.UpdateSystemSettingKeyValue(systemSetting);

            await adminActionNotificationHelper.CreateNotification<String>(user, EAdminAction.Update, "SystemSetting", $"KeyValue: {oldData}", $"KeyValue: {systemSettingDTO.KeyValue}");
            return "Cập nhập cài đặt hệ thông thành công.";
        }
    }
}
