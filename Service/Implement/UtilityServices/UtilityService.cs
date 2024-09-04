using BusinessObjects.DTOs;
using BusinessObjects.Enum;
using CloudinaryDotNet;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Service.Domain;
using Service.Implement.SystemService;
using Service.Interface.SystemServices;
using Service.Interface.UtilityServices;
using System.Globalization;
using System.Net;
using System.Web;

namespace Service.Implement.UtilityServices
{
    public class UtilityService : IUtilityService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISystemSettingService _systemSettingService = new SystemSettingService();

        #region SearchCity
        public IEnumerable<string> GetCitiesWithCountry(string keyword)
        {
            _loggerService.Information("Start to get City: ");
            var cities = new List<string>();
            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "cities.json").Replace("AdFusionAPI", "Service");

            string json = File.ReadAllText(inputFilePath);

            JArray countriesArray = JArray.Parse(json);

            var results = SearchCities(countriesArray, keyword);

            if (results.Any())
            {
                foreach (var result in results)
                {
                    cities.Add($"{result.Name}, {result.Country}");
                }
            }
            return cities;
        }
        static IEnumerable<CityResult> SearchCities(JArray countriesArray, string searchTerm)
        {
            // Tìm kiếm thành phố dựa trên chuỗi đầu vào với phương thức StartsWith cho phép so sánh không phân biệt chữ hoa chữ thường và hỗ trợ tiếng Việt
            _loggerService.Information("Start to search City: " + searchTerm);
            var results = countriesArray
                .SelectMany(country => country["cities"]!
                    .Select(city => new CityResult
                    {
                        Name = city["name"]!.ToString(),
                        Country = country["name"]!.ToString()
                    })
                    .Where(cityResult => cityResult.Name.StartsWith(searchTerm, true, new CultureInfo("vi-VN"))));

            return results;
        }
        #endregion

        public async Task<string> GetChannelProfile(int platform, string channelId)
        {
            switch ((EPlatform)platform)
            {
                case EPlatform.Tiktok:
                    var tiktokUrl = _configManager.TikTokUrl + channelId;
                    return await GetTikTokInformation(tiktokUrl);
                case EPlatform.Youtube:
                    return await GetYoutubeInformation(channelId);
                case EPlatform.Instagram:
                    var instagramUrl = _configManager.InstagramUrl + channelId;
                    return await GetInstagramInformation(instagramUrl);
                default:
                    throw new Exception("GetChannelProfile: Invalid input!");
            }
        }

        public async Task<string> GetVideoInformation(int platform, string url)
        {
            string decodedUrl = WebUtility.UrlDecode(url);
            switch ((EPlatform)platform)
            {
                case EPlatform.Tiktok:
                    return await GetVideoTikTokInformation(decodedUrl);
                case EPlatform.Youtube:
                    return await GetYoutubeVideoInformation(decodedUrl);
                case EPlatform.Instagram:
                    return await GetVideoInstagramInformation(decodedUrl);
                default:
                    throw new Exception("GetVideoInformation: Invalid input!");
            }
        }

        #region TikTok
        public async Task<string> GetTikTokInformation(string url)
        {
            _loggerService.Information("Start to get TikTok Account information: " + url);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            string decodedUrl = HttpUtility.UrlDecode(url);
            if (!decodedUrl.Contains("tiktok"))
            {
                throw new InvalidOperationException("Đường dẫn không hợp lệ");
            }
            var response = await client.GetStringAsync(decodedUrl);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var followerNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@id='__UNIVERSAL_DATA_FOR_REHYDRATION__']");

            string result = null!;

            if (followerNode != null)
            {
                string jsonContent = followerNode.InnerText;

                var jsonObj = JObject.Parse(jsonContent);

                result = jsonObj["__DEFAULT_SCOPE__"]?["webapp.user-detail"]?["userInfo"]?["stats"]?.ToString()!;
            }
            return result ?? throw new KeyNotFoundException();
        }
        public async Task<string> GetVideoTikTokInformation(string url)
        {
            try
            {
                _loggerService.Information("Start to get video TikTok information: " + url);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                string decodedUrl = HttpUtility.UrlDecode(url);
                if (!decodedUrl.Contains("tiktok"))
                {
                    throw new InvalidOperationException("Đường dẫn không hợp lệ");
                }
                var response = await client.GetStringAsync(decodedUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var followerNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@id='__UNIVERSAL_DATA_FOR_REHYDRATION__']");

                string result = null!;

                if (followerNode != null)
                {
                    string jsonContent = followerNode.InnerText;

                    var jsonObj = JObject.Parse(jsonContent);

                    result = jsonObj["__DEFAULT_SCOPE__"]?["webapp.video-detail"]?["itemInfo"]?.ToString()!;
                }

                return result ?? throw new KeyNotFoundException();
            }
            catch
            {
                throw new KeyNotFoundException();
            }
        }
        #endregion

        #region Instagram
        public async Task<string> GetVideoInstagramInformation(string url)
        {
            try
            {
                _loggerService.Information("Start to get video Instagram information: ");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                string decodedUrl = HttpUtility.UrlDecode(url);
                if (!decodedUrl.Contains("instagram"))
                {
                    throw new InvalidOperationException("Đường dẫn không hợp lệ");
                }
                var response = await client.GetStringAsync(decodedUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var followersNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");

                string result = null!;

                if (followersNode != null)
                {
                    string content = followersNode.GetAttributeValue("content", "");
                    string[] parts = content.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var data = new
                    {
                        likeCount = ConvertToNumber(parts[0]),
                        commentCount = ConvertToNumber(parts[2]),
                        actor = parts[5],
                        date = parts[7] + " " + parts[8] + " " + parts[9],
                    };
                    result = JsonConvert.SerializeObject(data)!;
                }

                return result ?? throw new KeyNotFoundException();
            }
            catch
            {
                throw new KeyNotFoundException();
            }
        }
        public async Task<string> GetInstagramInformation(string url)
        {
            _loggerService.Information("Start to get Instagram Account information: ");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            string decodedUrl = HttpUtility.UrlDecode(url);
            if (!decodedUrl.Contains("instagram"))
            {
                throw new InvalidOperationException("Đường dẫn không hợp lệ");
            }
            var response = await client.GetStringAsync(decodedUrl);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var followersNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
           
            string result = null!;

            if (followersNode != null)
            {
                string content = followersNode.GetAttributeValue("content", "");
                string[] parts = content.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var data = new
                {
                    followers = ConvertToNumber(parts[0]),
                    following = ConvertToNumber(parts[2]),
                    posts = ConvertToNumber(parts[4]),
                };
                result = JsonConvert.SerializeObject(data);
            }
            return result ?? throw new KeyNotFoundException();
        }
        #endregion

        #region Youtube
        public async Task<string> GetYoutubeInformation(string channelName)
        {
            try
            {
                var channelId = string.Empty;
                var apiKey = _systemSettingService.GetSystemSetting(_configManager.YoutubeAPIKey).Result.KeyValue;
                var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={channelName}&type=channel&key={apiKey}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var json = JObject.Parse(response);
                    channelId = json["items"]?[0]?["id"]?["channelId"]?.ToString();
                }

                var informationUrl = $"https://www.googleapis.com/youtube/v3/channels?part=statistics&id={channelId}&key={apiKey}";
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(informationUrl);
                    var json = JObject.Parse(response);
                    var subscriberCount = json["items"]?[0]?["statistics"]?.ToString();
                    return subscriberCount ?? throw new KeyNotFoundException();
                }
            }
            catch
            {
                throw new KeyNotFoundException();
            }
        }

        public async Task<string> GetYoutubeVideoInformation(string videoUrl)
        {
            try
            {
                var apiKey = _systemSettingService.GetSystemSetting(_configManager.YoutubeAPIKey).Result.KeyValue;
                string videoId = videoUrl.Substring(videoUrl.IndexOf("v=") + 2).Split('&')[0];

                string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=statistics&id={videoId}&key={apiKey}";

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(apiUrl);
                var json = JObject.Parse(response);

                if (json["items"] != null && json["items"].HasValues)
                {
                    return json["items"]?[0]?["statistics"]?.ToString()!;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            catch
            {
                throw new KeyNotFoundException();
            }
        }

        #endregion

        public static long ConvertToNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input không hợp lệ.");
            }

            // Loại bỏ dấu phẩy (,) nếu có để chuyển đổi đúng
            input = input.Replace(",", "");

            // Nếu đầu vào là "0"
            if (input == "0")
            {
                return 0;
            }

            // Kiểm tra ký tự cuối cùng để xác định đơn vị (K, M)
            char lastChar = input[input.Length - 1];

            if (char.IsLetter(lastChar))
            {
                // Nếu ký tự cuối cùng là chữ cái (K, M), ta cắt phần số
                string numberPart = input.Substring(0, input.Length - 1);
                double number = double.Parse(numberPart);

                switch (lastChar)
                {
                    case 'M':
                        return (long)(number * 1_000_000);
                    case 'K':
                        return (long)(number * 1_000);
                    default:
                        throw new ArgumentException("Đơn vị không hợp lệ.");
                }
            }
            else
            {
                // Nếu không có ký tự chữ cái, chỉ cần chuyển đổi thành số
                return long.Parse(input);
            }
        }

    }
}
