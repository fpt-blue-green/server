using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Service.Helper;
using System.Text.RegularExpressions;

namespace Service
{
    public class CampaignMeetingRoomService : ICampaignMeetingRoomService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();
        private static ICampaignMeetingRoomRepository _campaignMeetingRoomRepository = new CampaignMeetingRoomRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task<IEnumerable<CampaignMeetingRoomDTO>> GetMeetingRooms(Guid campaignId)
        {
            var result = await _campaignMeetingRoomRepository.GetMeetingRoomByCampaignId(campaignId);
            return _mapper.Map<IEnumerable<CampaignMeetingRoomDTO>>(result);
        }

        public async Task CreateRoom(RoomDataRequest dataRequest, UserDTO user)
        {
            ValidateCreateRoom(dataRequest);

            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);

            var roomData = new RoomSettingsDto
            {
                Name = dataRequest.Name,
                Properties = new RoomProperties
                {
                    Nbf = ConvertToUnixTimestamp(dataRequest.StartAt),
                    Exp = ConvertToUnixTimestamp(dataRequest.EndAt),
                    EjectAtRoomExp = true
                },
            };

            var link = await dailyVideoCall.CreateRoomAsync(roomData);

            var meetingRoom = new CampaignMeetingRoom
            {
                CampaignId = dataRequest.CampaignId!.Value,
                RoomName = GenerateUniqueJobName(dataRequest.Name),
                CreatedBy = user.Name!,
                Description = dataRequest.Description,
                StartAt = dataRequest.StartAt,
                EndAt = dataRequest.EndAt,
                RoomLink = link,
                Participants = string.Join(",", dataRequest.Participators),
            };
            await _campaignMeetingRoomRepository.CreateMeetingRoom(meetingRoom);

            await SendMail(meetingRoom, dataRequest.CampaignName);
        }

        public async Task CreateFirstTimeRoom( Guid campaignId)
        {
            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            var campaignName = "PhongHop";
            var roomData = new RoomSettingsDto
            {
                Name = GenerateUniqueJobName(campaignName),
                Properties = new RoomProperties
                {
                    Nbf = ConvertToUnixTimestamp(DateTime.Now),
                    Exp = null,
                    EjectAtRoomExp = false
                },
            };

            var link = await dailyVideoCall.CreateRoomAsync(roomData);

            var meetingRoom = new CampaignMeetingRoom
            {
                CampaignId = campaignId,
                RoomName = GenerateUniqueJobName(campaignName),
                CreatedBy = "System",
                RoomLink = link,
                StartAt = DateTime.Now,
                IsFirstTime = true,
            };
            await _campaignMeetingRoomRepository.CreateMeetingRoom(meetingRoom);
        }

        public async Task<(byte[] fileContent, string fileName)> GetLogFile(string roomName)
        {
            if (Regex.IsMatch(roomName, _configManager.DailyVideoNameRegex))
            {
                throw new Exception();
            }
            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            var data = await dailyVideoCall.FetchRoomLogAsync(roomName);
            return (data, $"{roomName}_{DateTime.Now.ToString("dd.MM.yyyy")}.csv");
        }

        public async Task DeleteRoomAsync(string roomName)
        {
            if (Regex.IsMatch(roomName, _configManager.DailyVideoNameRegex))
            {
                throw new Exception("Tên phòng không hợp lệ! Chỉ cho phép chữ cái, số, dấu gạch dưới _ và dấu gạch nối - ");
            }

            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            await dailyVideoCall.DeleteRoomAsync(roomName);

            var room = await _campaignMeetingRoomRepository.GetMeetingRoomByName(roomName) ?? throw new KeyNotFoundException();
            await _campaignMeetingRoomRepository.DeleteMeetingRoom(room);
        }

        #region SupportMethod
        protected static long ConvertToUnixTimestamp(DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

        protected static void ValidateCreateRoom(RoomDataRequest dataRequest)
        {
            if(dataRequest.StartAt < DateTime.Now)
            {
                throw new InvalidOperationException("Thời gian bắt đầu phải nhỏ hơn thời gian hiện tại.");
            }

            if (dataRequest.StartAt.AddHours(2) >= dataRequest.EndAt)
            {
                throw new InvalidOperationException("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc ít nhất là 2 tiếng.");
            }

            if (Regex.IsMatch(dataRequest.Name, _configManager.DailyVideoNameRegex))
            {
                throw new InvalidOperationException("Tên phòng không hợp lệ! Chỉ cho phép chữ cái, số, dấu gạch dưới _ và dấu gạch nối - .");
            }

            if (!dataRequest.Participators.Any())
            {
                throw new InvalidOperationException("Để tạo cuộc họp, ít nhất cần phải có một người tham gia (ngoại trừ chủ phòng).");
            }
        }

        protected static string GenerateUniqueJobName(string name)
        {
            Random random = new Random();

            // Lấy thời gian hiện tại
            DateTime now = DateTime.Now;

            // Định dạng chuỗi với ngày tháng năm và giờ phút giây
            string formattedDate = now.ToString("yyyyMMdd"); // 20241024
            string formattedTime = now.ToString("HHmmss");   // 032040

            // Tạo số ngẫu nhiên từ 10000 đến 99999
            int randomNumber = random.Next(10000, 99999);

            // Kết hợp lại thành chuỗi duy nhất
            return $"{name}_{formattedDate}_{formattedTime}_{randomNumber}";
        }

        public async Task SendMail(CampaignMeetingRoom campaignMeeting, string campaginName)
        {
            try
            {
                string subject = " Thông Báo Cuộc Họp Sắp Diễn Ra";
                var emails = campaignMeeting.Participants.Split(',')
                         .Select(email => email.Trim())
                         .Where(email => !string.IsNullOrEmpty(email))
                         .ToList() ?? throw new Exception();

                var body = _emailTemplate.meetingNotificationTemplate
                    .Replace("{BrandName}", campaignMeeting.CreatedBy)
                    .Replace("{CampaignName}", campaginName)
                    .Replace("{StartTime}", campaignMeeting.StartAt.ToString("dd-MM-yyyy HH:mm:ss"))
                    .Replace("{EndTime}", campaignMeeting.EndAt!.Value.ToString("dd-MM-yyyy HH:mm:ss"))
                    .Replace("{Description}", campaignMeeting.Description)
                    .Replace("{MeetingLink}", campaignMeeting.RoomLink)
                    .Replace("{ProjectName}", _configManager.ProjectName);

                _ = Task.Run(async () => await _emailService.SendEmail(emails, subject, body));
            }
            catch
            {
                throw new InvalidOperationException("Gửi mail thất bại. Vui lòng tạo mới  cuộc họp hoặc gửi lại thông báo cho nhà sáng tạo nội dung.");
            }
        }
        #endregion
    }
}
