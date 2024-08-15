using BusinessObjects.ModelsDTO;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Service.Domain;
using Service.Interface;
using System.Globalization;
using System.Web;

namespace Service.Implement
{
    public class UtilityService : IUtilityService
    {
        private static ConfigManager _configManager = new ConfigManager();
        private static ILogger _loggerService = new LoggerService().GetLogger();

        public IEnumerable<string> GetCitiesWithCountry(string keyword)
        {
            try
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
                    return cities;
                }
                else
                {
                    throw new Exception("Không tìm thấy thành phố phù hợp.");
                }
            }
            catch (Exception ex)
            {
                _loggerService.Information(ex.ToString());
                return null!;
            }
        }

        static IEnumerable<CityResult> SearchCities(JArray countriesArray, string searchTerm)
        {
            // Tìm kiếm thành phố dựa trên chuỗi đầu vào với phương thức StartsWith cho phép so sánh không phân biệt chữ hoa chữ thường và hỗ trợ tiếng Việt
            _loggerService.Information("Start to search City: ");
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

        public async Task<string> GetTikTokInformation(string url)
        {
            try
            {
                _loggerService.Information("Start to get TikTok Account information: ");
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

                throw new Exception("Không tìm thấy thông tin tài khoản.");
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                return string.Empty;
            }
        }

        public async Task<string> GetVideoTikTokInformation(string url)
        {
            try
            {
                _loggerService.Information("Start to get video TikTok information: ");
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

                throw new Exception("Không tìm thấy thông tin video.");
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                return string.Empty;
            }
        }

        public async Task<string> GetVideoInstagramInformation(string url)
        {
            try
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

                throw new Exception("Không tìm thấy thông tin video.");
            }
            catch(Exception ex)
            {
                _loggerService.Error(ex.ToString());
                return string.Empty;
            }
        }

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

        public async Task<string> GetInstagramInformation(string url)
        {
            try
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

                throw new Exception("Không tìm thấy thông tin account.");
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                return string.Empty;
            }
        }
    }
}
