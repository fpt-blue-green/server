
using BusinessObjects;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Globalization;
using System.Net;
using System.Web;

namespace Service
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

        public async Task<ChannelStatDTO> GetChannelProfile(int platform, string channelId)
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
        public async Task<ChannelStatDTO> GetTikTokInformation(string url)
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

            JObject result = null!;

            if (followerNode != null)
            {
                string jsonContent = followerNode.InnerText;

                var jsonObj = JObject.Parse(jsonContent);

                result = (JObject)jsonObj["__DEFAULT_SCOPE__"]?["webapp.user-detail"]?["userInfo"]?["stats"]!;
            }

            if(result == null)
            {
                throw new KeyNotFoundException();
            }

            return new ChannelStatDTO
            {
                FollowersCount = (int?)result!["followerCount"],
                LikesCount = (int?)result["heartCount"],
                PostsCount = (int?)result["videoCount"],
            };
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
        public async Task<ChannelStatDTO> GetInstagramInformation(string url)
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
                var data = new ChannelStatDTO
                {
                    FollowersCount = int.Parse(parts[0]),
                    LikesCount = int.Parse(parts[2]),
                    PostsCount = int.Parse(parts[4])              
                };
                return data ?? throw new KeyNotFoundException();
            }
            throw new KeyNotFoundException();
        }
        #endregion

        #region Youtube
        public async Task<ChannelStatDTO> GetYoutubeInformation(string channelName)
        {
            try
            {
                var channelId = string.Empty;

                // Lấy API Key từ hệ thống cài đặt
                var apiKey = _systemSettingService.GetSystemSetting(_configManager.YoutubeAPIKey).Result.KeyValue;

                // Tạo URL để tìm kiếm kênh bằng tên
                var searchUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={channelName}&type=channel&key={apiKey}";

                // Tìm kiếm kênh YouTube bằng tên
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(searchUrl);
                    var json = JObject.Parse(response);
                    channelId = json["items"]?[0]?["id"]?["channelId"]?.ToString();
                }

                // Tạo URL để lấy thông tin về kênh
                var informationUrl = $"https://www.googleapis.com/youtube/v3/channels?part=statistics&id={channelId}&key={apiKey}";

                // Lấy thông tin số liệu từ API YouTube
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(informationUrl);
                    var json = JObject.Parse(response);
                    var statistics = json["items"]?[0]?["statistics"];

                    // Nếu không tìm thấy thông tin thống kê, ném ra ngoại lệ
                    if (statistics == null)
                    {
                        throw new KeyNotFoundException();
                    }

                    // Tạo đối tượng ChannelStatDTO với các dữ liệu từ API
                    return new ChannelStatDTO
                    {
                        FollowersCount = (int?)statistics["subscriberCount"],
                        ViewsCount = (int?)statistics["viewCount"],
                        PostsCount = (int?)statistics["videoCount"],
                    };
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
                var apiKey = await _systemSettingService.GetSystemSetting(_configManager.YoutubeAPIKey);
                string videoId = videoUrl.Substring(videoUrl.IndexOf("v=") + 2).Split('&')[0];

                string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=statistics,snippet&id={videoId}&key={apiKey.KeyValue}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(apiUrl);
                    var json = JObject.Parse(response);

                    // Check if items are present and have values
                    if (json["items"] != null && json["items"].HasValues)
                    {
                        // Check if the video is live
                        var item = json["items"][0];
                        var liveBroadcastContent = item["snippet"]?["liveBroadcastContent"]?.ToString();

                        if (liveBroadcastContent == "live")
                        {
                            // For livestream, you might want to check additional stats or info
                            return $"Live stream statistics: {item["statistics"]?.ToString()!}";
                        }
                        else
                        {
                            // For regular videos
                            return $"Video statistics: {item["statistics"]?.ToString()!}";
                        }
                    }
                    else
                    {
                        throw new KeyNotFoundException("No items found in the response.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle specific exception details
                throw new Exception("An error occurred while retrieving YouTube video information.", ex);
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
