using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Service.Helper;

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

        public async Task<IEnumerable<SystemSettingDTO>> GetSystemSettings()
        {
            var result = await _systemSettingRepository.GetAll();
            return _mapper.Map<IEnumerable<SystemSettingDTO>>(result);
        }

        public async Task<SystemSettingDTO> GetSystemSetting(string keyName)
        {
            var data = await _systemSettingRepository.GetSystemSetting(keyName);
            var result = _mapper.Map<SystemSettingDTO>(data);
            return result;
        }

        public async Task UpdateSystemSetting(SystemSettingDTO systemSettingDTO, UserDTO user)
        {
            var systemSetting = await _systemSettingRepository.GetSystemSetting(systemSettingDTO.KeyName);
            var oldKeyData = systemSetting.KeyValue;
            var oldDesData = systemSetting.Description;
            if (systemSetting == null)
            {
                throw new KeyNotFoundException();
            }
            systemSetting.KeyValue = systemSettingDTO.KeyValue;
            systemSetting.Description = systemSettingDTO.Description;
            await _systemSettingRepository.UpdateSystemSettingKeyValue(systemSetting);

            await adminActionNotificationHelper.CreateNotification<String>(user, EAdminActionType.Update,
                                                    $"KeyValue: {oldKeyData}, Description: {oldDesData}",
                                                    $"KeyValue: {systemSettingDTO.KeyValue}, Description: {systemSettingDTO.Description}",
                                                    "SystemSetting");
        }
    }
}
