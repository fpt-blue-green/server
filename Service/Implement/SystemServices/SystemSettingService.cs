using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{

    public class SystemSettingService : ISystemSettingService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();

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
