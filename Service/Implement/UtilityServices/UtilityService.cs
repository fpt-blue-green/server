
using BusinessObjects;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Serilog;
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

            var results = SearchCities(countriesArray, keyword).Take(50);

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
                     .Where(cityResult => cityResult.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)));

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

        public async Task<ChannelVideoStatDTO> GetVideoInformation(int platform, string url)
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

            if (result == null)
            {
                throw new KeyNotFoundException();
            }

            return new ChannelStatDTO
            {
                FollowersCount = ConvertToNumber(result!["followerCount"]?.ToString() ?? "0"),
                LikesCount = ConvertToNumber(result!["heartCount"]?.ToString() ?? "0"),
                PostsCount = ConvertToNumber(result!["videoCount"]?.ToString() ?? "0")
            };
        }
        public async Task<ChannelVideoStatDTO> GetVideoTikTokInformation(string url)
        {
            try
            {
                _loggerService.Information("Start to get video TikTok information: " + url);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                string decodedUrl = HttpUtility.UrlDecode(url);
                if (!decodedUrl.Contains("tiktok"))
                {
                    throw new InvalidOperationException("Đường dẫn không hợp lệ");
                }

                var response = await client.GetStringAsync(decodedUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var followerNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@id='__UNIVERSAL_DATA_FOR_REHYDRATION__']");

                if (followerNode == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy dữ liệu TikTok.");
                }

                string jsonContent = followerNode.InnerText;
                var jsonObj = JObject.Parse(jsonContent);

                var videoInfo = jsonObj["__DEFAULT_SCOPE__"]?["webapp.video-detail"]?["itemInfo"]?["itemStruct"];

                if (videoInfo == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy thông tin video.");
                }

                var data = new ChannelVideoStatDTO
                {
                    ViewsCount = ConvertToNumber(videoInfo["stats"]?["playCount"]?.ToString() ?? "0"),
                    LikesCount = ConvertToNumber(videoInfo["stats"]?["diggCount"]?.ToString() ?? "0"),
                    CommentCount = ConvertToNumber(videoInfo["stats"]?["commentCount"]?.ToString() ?? "0"),
                };

                return data;
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex, "Failed to get video TikTok information");
                throw new KeyNotFoundException("Không tìm thấy thông tin video.");
            }
        }

        #endregion

        #region Instagram
        public async Task<ChannelVideoStatDTO> GetVideoInstagramInformation(string url)
        {
            try
            {
                _loggerService.Information("Start to get video Instagram information: " + url);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                string decodedUrl = HttpUtility.UrlDecode(url);
                if (!decodedUrl.Contains("instagram"))
                {
                    throw new InvalidOperationException("Đường dẫn không hợp lệ");
                }

                var response = await client.GetStringAsync(decodedUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                string? viewCount = null;
                string? likeCount = null;
                string? commentCount = null;

                // Lấy số lượt xem từ thẻ meta
                var viewCountNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:video:view_count']");
                if (viewCountNode != null)
                {
                    viewCount = viewCountNode.GetAttributeValue("content", "0");
                }

                // Lấy số lượt thích và bình luận từ thẻ description
                var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
                if (descriptionNode != null)
                {
                    string content = descriptionNode.GetAttributeValue("content", "");
                    string[] parts = content.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 9)
                    {
                        likeCount = ConvertToNumber(parts[0]).ToString();
                        commentCount = ConvertToNumber(parts[2]).ToString();
                    }
                }

                var data = new ChannelVideoStatDTO
                {
                    ViewsCount = ConvertToNumber(viewCount ?? "0"),
                    LikesCount = ConvertToNumber(likeCount ?? "0"),
                    CommentCount = ConvertToNumber(commentCount ?? "0")
                };

                return data;
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex, "Failed to get video Instagram information");
                throw new KeyNotFoundException("Không tìm thấy thông tin video.");
            }
        }

        public async Task<ChannelStatDTO> GetInstagramInformation(string url)
        {
            _loggerService.Information("Start to get Instagram Account information: " + url);
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


            if (followersNode != null)
            {
                string content = followersNode.GetAttributeValue("content", "");
                string[] parts = content.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var data = new ChannelStatDTO
                {
                    FollowersCount = ConvertToNumber(parts[0]),
                    LikesCount = ConvertToNumber(parts[2]),
                    PostsCount = ConvertToNumber(parts[4])
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
                _loggerService.Information("Start to get video Youtube information: " + channelName);
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

        public async Task<ChannelVideoStatDTO> GetYoutubeVideoInformation(string videoUrl)
        {
            try
            {
                _loggerService.Information("Start to get video Youtube Video information: " + videoUrl);
                var apiKey = await _systemSettingService.GetSystemSetting(_configManager.YoutubeAPIKey);
                string videoId = videoUrl.Substring(videoUrl.IndexOf("v=") + 2).Split('&')[0];

                string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=statistics,snippet&id={videoId}&key={apiKey.KeyValue}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(apiUrl);
                    var json = JObject.Parse(response);

                    // Check if items are present and have values
                    if (json["items"] != null && json["items"]!.HasValues)
                    {
                        // Check if the video is live
                        var item = json["items"]![0];
                        var liveBroadcastContent = item!["snippet"]?["liveBroadcastContent"]?.ToString();

                        var statistics = item["statistics"];
                        var data = new ChannelVideoStatDTO
                        {
                            ViewsCount = ConvertToNumber(statistics?["viewCount"]?.ToString() ?? "0"),
                            LikesCount = ConvertToNumber(statistics?["likeCount"]?.ToString() ?? "0"),
                            CommentCount = ConvertToNumber(statistics?["commentCount"]?.ToString() ?? "0")
                        };
                        return data;
                    }
                    else
                    {
                        throw new KeyNotFoundException("No items found in the response.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving YouTube video information.", ex);
            }
        }
        #endregion

        public static int ConvertToNumber(string input)
        {
            input = input.Replace(",", "");

            if (input == "0")
            {
                return 0;
            }

            char lastChar = input[input.Length - 1];

            if (char.IsLetter(lastChar))
            {
                string numberPart = input.Substring(0, input.Length - 1);
                if (!double.TryParse(numberPart, out double number))
                {
                    throw new Exception();
                }

                switch (lastChar)
                {
                    case 'M':
                        return (int)(number * 1_000_000);
                    case 'K':
                        return (int)(number * 1_000);
                    case 'N':
                        return (int)(number * 1_000);
                    default:
                        throw new ArgumentException("Đơn vị không hợp lệ.");
                }
            }
            else
            {
                if (!int.TryParse(input, out int result))
                {
                    throw new Exception();
                }
                return result;
            }
        }


    }
}
