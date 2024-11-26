using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Service.Helper;
using System;
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
        private static IBrandRepository _brandRepository = new BrandRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task<IEnumerable<CampaignMeetingRoomDTO>> GetMeetingRooms(Guid campaignId)
        {
            var meetingRooms = await _campaignMeetingRoomRepository.GetMeetingRoomsByCampaignId(campaignId);
            return _mapper.Map<IEnumerable<CampaignMeetingRoomDTO>>(meetingRooms);
        }

        public async Task CreateRoom(RoomDataRequest dataRequest, UserDTO user)
        {
            if (dataRequest.StartAt.Kind != DateTimeKind.Utc || dataRequest.EndAt.Kind != DateTimeKind.Utc)
            {
                dataRequest.StartAt = dataRequest.StartAt.ToUniversalTime();
                dataRequest.StartAt = dataRequest.StartAt.ToUniversalTime();
            }
            await ValidateCreateRoom(dataRequest.StartAt, dataRequest.EndAt, dataRequest.RoomName, dataRequest.Participators, user.Id);

            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);

            var roomName = GenerateUniqueJobName(dataRequest.RoomName);

            var roomData = new RoomSettingsDto
            {
                Name = roomName,
                Properties = new RoomProperties
                {
                    Nbf = ConvertToUnixTimestamp(dataRequest.StartAt),
                    Exp = ConvertToUnixTimestamp(dataRequest.EndAt),
                    EjectAtRoomExp = true,
                    EnableKnocking = true,
                },
                Privacy = "private"
            };

            var link = await dailyVideoCall.CreateRoomAsync(roomData);

            var meetingRoom = new CampaignMeetingRoom
            {
                CampaignId = dataRequest.CampaignId!.Value,
                RoomName = roomName,
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

        public async Task CreateFirstTimeRoom(Guid campaignId)
        {
            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            var campaignName = "PhongHop";

            var roomData = new RoomSettingsDto
            {
                Name = GenerateUniqueJobName(campaignName),
                Properties = new RoomProperties
                {
                    Nbf = ConvertToUnixTimestamp(DateTime.UtcNow),
                    Exp = null,
                    EjectAtRoomExp = true,
                },
                Privacy = "public"
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

        public async Task UpdateRoom(RoomDataUpdateRequest updateRequest, UserDTO user)
        {
            if (updateRequest.StartAt.Kind != DateTimeKind.Utc || updateRequest.EndAt.Kind != DateTimeKind.Utc)
            {
                updateRequest.StartAt = updateRequest.StartAt.ToUniversalTime();
                updateRequest.StartAt = updateRequest.StartAt.ToUniversalTime();
            }
            await ValidateCreateRoom(updateRequest.StartAt, updateRequest.EndAt, updateRequest.RoomName, updateRequest.Participators, user.Id);

            var curRoom = await _campaignMeetingRoomRepository.GetMeetingRoomByName(updateRequest.RoomName) ?? throw new KeyNotFoundException();

            if (curRoom.IsFirstTime)
            {
                throw new InvalidOperationException("Không thể chỉnh sửa phòng mặc định của chiến dịch.");
            }

            curRoom.StartAt = updateRequest.StartAt;
            curRoom.EndAt = updateRequest.EndAt;
            curRoom.Description = updateRequest.Description;
            curRoom.Participants = string.Join(",", updateRequest.Participators);

            await _campaignMeetingRoomRepository.UpdateMeetingRoom(curRoom);

            var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");

            //Delete old Room in the Daily
            DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
            await dailyVideoCall.DeleteRoomAsync(curRoom.RoomName);

            //Create new Room in Daily
            var roomData = new RoomSettingsDto
            {
                Name = curRoom.RoomName,
                Properties = new RoomProperties
                {
                    Nbf = ConvertToUnixTimestamp(updateRequest.StartAt),
                    Exp = ConvertToUnixTimestamp(updateRequest.EndAt),
                    EjectAtRoomExp = true,
                    EnableKnocking = true,
                },
                Privacy = "private"
            };

            await dailyVideoCall.CreateRoomAsync(roomData);

            await SendMail(curRoom, curRoom.Campaign.Name);
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

        public async Task<string> GetAccessLinkByRole(string roomName, UserDTO userDTO)
        {
            if (Regex.IsMatch(roomName, _configManager.DailyVideoNameRegex))
            {
                throw new Exception("Tên phòng không hợp lệ! Chỉ cho phép chữ cái, số, dấu gạch dưới _ và dấu gạch nối - ");
            }

            var result = await _campaignMeetingRoomRepository.GetMeetingRoomByName(roomName) ?? throw new Exception();

            if (userDTO.Role == AuthEnumContainer.ERole.Influencer || result.IsFirstTime)
            {
                return result.RoomLink;
            }
            else
            {
                var apiKey = await _systemSettingRepository.GetSystemSetting(_configManager.DailyVideoCallKey) ?? throw new Exception("Has error when get API VIDEO CALL Key");
                DailyVideoCallHelper dailyVideoCall = new DailyVideoCallHelper(apiKey.KeyValue!);
                var token = await dailyVideoCall.GetOwnerTokenAsync(roomName);
                return result.RoomLink + $"?t={token}";
            }
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

        protected static async Task ValidateCreateRoom(DateTime startAt, DateTime endAt, string roomName, List<string> participators, Guid id)
        {
            if (startAt.Kind == DateTimeKind.Utc)
            {
                startAt = startAt.AddHours(7);
                endAt = endAt.AddHours(7);
            }

            if (startAt < DateTime.Now.AddMinutes(-5))
            {
                throw new InvalidOperationException("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");
            }

            if (startAt.AddHours(2) >= endAt)
            {
                throw new InvalidOperationException("Thời gian của cuộc gọi tối thiểu là 2 tiếng.");
            }

            if (Regex.IsMatch(roomName, _configManager.DailyVideoNameRegex))
            {
                throw new InvalidOperationException("Tên phòng không hợp lệ! Chỉ cho phép chữ cái, số, dấu gạch dưới _ và dấu gạch nối - .");
            }

            if (participators.Count > 1)
            {
                throw new InvalidOperationException("Để tạo cuộc họp, ít nhất cần phải có một người tham gia (ngoại trừ chủ phòng).");
            }

            var user = await _brandRepository.GetByUserId(id) ?? throw new Exception();

            //if (user.IsPremium == false)
            //{
            //    throw new AccessViolationException();
            //}

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
