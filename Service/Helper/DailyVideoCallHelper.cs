using Newtonsoft.Json;
using OfficeOpenXml;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static BusinessObjects.VideoCallSessionModelDTO;

namespace Service.Helper
{
    public class DailyVideoCallHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _roomEndpoint;
        private static ConfigManager _configManager = new ConfigManager();
        private static ILogger _logger = new LoggerService().GetDbLogger();

        public DailyVideoCallHelper(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _roomEndpoint = _configManager.DailyVideoCallEnpoint;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Tạo phòng họp mới
        public async Task<string> CreateRoomAsync(string roomName, long? expiryTime = null)
        {
            var roomData = new
            {
                name = roomName,
                properties = new
                {
                    exp = expiryTime
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(roomData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_roomEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"Failed to create room: {response.ReasonPhrase}");
                return string.Empty;
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var url = JsonDocument.Parse(responseData).RootElement.GetProperty("url").GetString();
            return url;
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

            //var logData = await response.Content.ReadAsStringAsync();
            string logData = "{\r\n    \"total_count\": 8,\r\n    \"data\": [\r\n        {\r\n            \"id\": \"5c242b1b-604c-4a10-b8dd-ff00cc10d023\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1728026936,\r\n            \"duration\": 53,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 2,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"53d1c4f3-5b94-429f-bf7d-b3e56a76048d\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1728026936,\r\n                    \"duration\": 53\r\n                },\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"d9d25667-f28a-40e7-a97b-a581681c4607\",\r\n                    \"user_name\": \"Ronaldo\",\r\n                    \"join_time\": 1728026954,\r\n                    \"duration\": 31\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"7c25a5af-4d89-4847-9d73-d4cc237d96db\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1728026741,\r\n            \"duration\": 171,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 1,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"fcf55fe1-3f93-40dd-87b7-c3bf63e27aac\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1728026741,\r\n                    \"duration\": 171\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"c6e92eeb-1fd5-4e54-96dd-25a2aec90475\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1728026679,\r\n            \"duration\": 15,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 1,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"dceb6611-8e61-45e1-aa90-429671c6c27e\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1728026679,\r\n                    \"duration\": 15\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"85aa0f7a-7bc0-4af3-be74-41be70e932b0\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1723451848,\r\n            \"duration\": 227,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 1,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"8e11e37a-30cd-47f5-b69d-7a7169b243fc\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1723451848,\r\n                    \"duration\": 227\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"8dbb86f8-34f7-4a3f-84d3-62f3e201981a\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1723104476,\r\n            \"duration\": 400,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 2,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"03bf8aa6-22f8-44b8-9f9f-c6f064d5e7d6\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1723104476,\r\n                    \"duration\": 400\r\n                },\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"14a6e0ad-70cb-4724-afcb-f488eed3983a\",\r\n                    \"user_name\": \"Syaoran\",\r\n                    \"join_time\": 1723104591,\r\n                    \"duration\": 281\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"ac63f699-d584-4ef5-9665-6d699a77fcdf\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1723020484,\r\n            \"duration\": 105,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 1,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"a99aaab9-989e-492a-9a5d-32b3fdb1d893\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1723020484,\r\n                    \"duration\": 105\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"00fb651b-6cb1-484a-92e2-bdabd9cedd0d\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1723019329,\r\n            \"duration\": 245,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 2,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"2936e38b-eec0-4aae-b952-a766405b114b\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1723019329,\r\n                    \"duration\": 245\r\n                },\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"65c015dc-44d7-484d-b309-419ab1156f08\",\r\n                    \"user_name\": \"Nice\",\r\n                    \"join_time\": 1723019353,\r\n                    \"duration\": 30\r\n                },\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"e3fea5e3-0bdc-4c6b-91e7-f708b1b482e6\",\r\n                    \"user_name\": \"ádsa\",\r\n                    \"join_time\": 1723019548,\r\n                    \"duration\": 15\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"id\": \"11ba63e2-63ad-4d39-92bd-1d8492e354b3\",\r\n            \"room\": \"7RfsBDVQUoC0EJXIzWQ3\",\r\n            \"start_time\": 1723018566,\r\n            \"duration\": 431,\r\n            \"ongoing\": false,\r\n            \"max_participants\": 2,\r\n            \"participants\": [\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"4a00f151-cf55-4b09-b23d-a5a1a6aa9d9c\",\r\n                    \"user_name\": \"Hello\",\r\n                    \"join_time\": 1723018658,\r\n                    \"duration\": 5\r\n                },\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"c2c675b3-6ae5-4118-98f1-0f656417c21c\",\r\n                    \"user_name\": \"Tyson\",\r\n                    \"join_time\": 1723018566,\r\n                    \"duration\": 431\r\n                },\r\n                {\r\n                    \"user_id\": null,\r\n                    \"participant_id\": \"eda3e840-3cdd-4646-87f9-87f7c0700113\",\r\n                    \"user_name\": \"fd\",\r\n                    \"join_time\": 1723018716,\r\n                    \"duration\": 78\r\n                }\r\n            ]\r\n        }\r\n    ]\r\n}";

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

