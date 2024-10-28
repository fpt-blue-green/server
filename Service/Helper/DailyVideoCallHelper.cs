using BusinessObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static BusinessObjects.VideoCallSessionModelDTO;

namespace Service.Helper
{
    public class DailyVideoCallHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _roomEndpoint;
        private readonly string _roomTookenEndpoint;
        private static ConfigManager _configManager = new ConfigManager();
        private static ILogger _logger = new LoggerService().GetDbLogger();

        public DailyVideoCallHelper(string apiKey)
        {
            _apiKey = apiKey;
            _roomEndpoint = _configManager.DailyVideoCallEnpoint;
            _roomTookenEndpoint = _configManager.DailyVideoCallTokenEnpoint;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Tạo phòng họp mới
        public async Task<string> CreateRoomAsync(RoomSettingsDto settings)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            };

            var content = new StringContent(JsonConvert.SerializeObject(settings, options), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_roomEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create room: {response.ReasonPhrase}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var url = JsonDocument.Parse(responseData).RootElement.GetProperty("url").GetString();
            return url ?? throw new Exception();
        }

        //  Lấy token cho Owner
        public async Task<string> GetOwnerTokenAsync(string roomName)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            };

            var requestBody = new
            {
                properties = new
                {
                    room_name = roomName,
                    is_owner = true
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody, options), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_roomTookenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorData = await response.Content.ReadAsStringAsync();
                throw new Exception("Failed to get owner token");
            }

            var responseData = JsonObject.Parse(await response.Content.ReadAsStringAsync());
            return responseData["token"]!.ToString();
        }

        // Xóa phòng họp
        public async Task<bool> DeleteRoomAsync(string roomName)
        {
            var response = await _httpClient.DeleteAsync($"{_roomEndpoint}/{roomName}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"Failed to delete room: {response.ReasonPhrase}");
                return false;
            }

            return true;
        }

        // Lấy log của phòng họp
        public async Task<byte[]> FetchRoomLogAsync(string roomName)
        {
            var response = await _httpClient.GetAsync($"https://api.daily.co/v1/meetings?room={roomName}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"Failed to fetch room log: {response.ReasonPhrase}");
                return null!;
            }

            var logData = await response.Content.ReadAsStringAsync();

            var result = CreateExcel(logData);
            return result;
        }

        static string ConvertToUTC7(string timestampStr)
        {
            var timestamp = long.Parse(timestampStr);
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            var utc7DateTime = dateTime.ToOffset(TimeSpan.FromHours(7));
            return utc7DateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static byte[] CreateExcel(string rawStringData)
        {
            var rawData = JsonConvert.DeserializeObject<DataSessionDTO>(rawStringData);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Tạo tệp Excel
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sessions");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Room";
                worksheet.Cells[1, 2].Value = "Start Time (UTC+7)";
                worksheet.Cells[1, 3].Value = "Duration (Seconds)";
                worksheet.Cells[1, 4].Value = "Ongoing";
                worksheet.Cells[1, 5].Value = "Max Participants (People)";
                worksheet.Cells[1, 6].Value = "User Name";
                worksheet.Cells[1, 7].Value = "Join Time (UTC+7)";
                worksheet.Cells[1, 8].Value = "Participant Duration (Seconds)";

                int row = 2;

                // Thêm dữ liệu
                foreach (var session in rawData.Sessions)
                {
                    // Chuyển đổi thông tin phiên
                    var room = session.Room;
                    var startTime = ConvertToUTC7(session.StartTime);
                    var duration = session.Duration;
                    var ongoing = session.Ongoing;
                    var maxParticipants = session.MaxParticipants;

                    foreach (var participant in session.Participants)
                    {
                        worksheet.Cells[row, 1].Value = room; // Room
                        worksheet.Cells[row, 2].Value = startTime; // Start Time
                        worksheet.Cells[row, 3].Value = duration; // Duration
                        worksheet.Cells[row, 4].Value = ongoing; // Ongoing
                        worksheet.Cells[row, 5].Value = maxParticipants; // Max Participants
                        worksheet.Cells[row, 6].Value = participant.UserName; // User Name
                        worksheet.Cells[row, 7].Value = ConvertToUTC7(participant.JoinTime); // Join Time
                        worksheet.Cells[row, 8].Value = participant.Duration; // Participant Duration
                        row++;
                    }
                }
                worksheet.Cells.AutoFitColumns();
                // Trả về mảng byte của tệp Excel
                return package.GetAsByteArray();
            }
        }
    }
}

