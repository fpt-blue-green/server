using Repositories;
using Service.Helper;
using System.Text;

namespace Service
{
    public class VideoCallService : IVideoCallService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();

        public async Task<string> CreateRoom(string roomName)
        {
            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            var link = await dailyVideoCall.CreateRoomAsync(roomName);

            return link;
        }

        public async Task<(byte[] fileContent, string fileName)> GetLogFile()
        {
            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            var data =  await dailyVideoCall.FetchRoomLogAsync("test123");
            return (data, "test.csv");
        }

        public async Task DeleteRoomAsync(string roomName)
        {
            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            await dailyVideoCall.DeleteRoomAsync(roomName);
        }
    }
}
