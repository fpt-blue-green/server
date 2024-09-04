using BusinessObjects.DTOs;
using BusinessObjects.Enum;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Service.Domain;
using Service.Implement.SystemService;
using Service.Interface.SystemServices;
using Service.Interface.UtilityServices;
using System.Globalization;
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

        #region TikTok
        public async Task<string> GetTikTokInformation(string url)
        {
            _loggerService.Information("Start to get TikTok Account information: " + url);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

            string decodedUrl = HttpUtility.UrlDecode(url);
            var response = await client.GetStringAsync(decodedUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var followerNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@id='__UNIVERSAL_DATA_FOR_REHYDRATION__']");

            if (followerNode != null)
            {
                string jsonContent = followerNode.InnerText;

                var jsonObj = JObject.Parse(jsonContent);

                var accountInfo = jsonObj["__DEFAULT_SCOPE__"]?["webapp.user-detail"]?["userInfo"]?.ToString();
                return accountInfo ?? string.Empty;
            }
            throw new InvalidOperationException("Không tìm thấy thông tin tài khoản.");
        }
        public async Task<string> GetVideoTikTokInformation(string url)
        {
            _loggerService.Information("Start to get video TikTok information: " + url);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

            string decodedUrl = HttpUtility.UrlDecode(url);
            var response = await client.GetStringAsync(decodedUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var followerNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@id='__UNIVERSAL_DATA_FOR_REHYDRATION__']");

            if (followerNode != null)
            {
                string jsonContent = followerNode.InnerText;

                var jsonObj = JObject.Parse(jsonContent);

                var videoInfo = jsonObj["__DEFAULT_SCOPE__"]?["webapp.video-detail"]?["itemInfo"]?.ToString();
                return videoInfo ?? string.Empty;
            }

            throw new InvalidOperationException("Không tìm thấy thông tin video.");
        }
        #endregion

        #region Instagram
        public async Task<string> GetVideoInstagramInformation(string url)
        {
            _loggerService.Information("Start to get video Instagram information: ");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

            string decodedUrl = HttpUtility.UrlDecode(url);
            var response = await client.GetStringAsync(decodedUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var followersNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");

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
                return JsonConvert.SerializeObject(data) ?? string.Empty;
            }
            throw new InvalidOperationException("Không tìm thấy thông tin tài khoản.");
        }
        public async Task<string> GetInstagramInformation(string url)
        {
            _loggerService.Information("Start to get Instagram Account information: ");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

            string decodedUrl = HttpUtility.UrlDecode(url);
            var response = await client.GetStringAsync(decodedUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var followersNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");

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
                return JsonConvert.SerializeObject(data) ?? string.Empty;
            }
            throw new InvalidOperationException("Không tìm thấy thông tin video.");
        }
        #endregion

        #region Youtube
        public async Task<string> GetYoutubeInformation(string channelName)
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
                return subscriberCount ?? throw new InvalidOperationException("Không tìm thấy thông tin tài khoản."); ;
            }
        }
        #endregion
        public static long ConvertToNumber(string input)
        {
            if (input == "0")
            {
                return 0;
            }

            char lastChar = input[input.Length - 1];

            string numberPart = input.Substring(0, input.Length - 1);
            double number = double.Parse(numberPart);

            switch (lastChar)
            {
                case 'M':
                    return (long)(number * 1_000_000);
                case 'K':
                    return (long)(number * 1_000);
                default:
                    return long.Parse(input);
            }
        }

    }
}
