using BusinessObjects.ModelsDTO;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Service.Domain;
using Service.Interface;
using System.Globalization;
using System.Web;

namespace Service.Implement
{
    public class UtilityService : IUtilityService
    {
        private static ConfigManager _configManager = new ConfigManager();

        public IEnumerable<string> GetCitiesWithCountry(string keyword)
        {
            try
            {
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
                    throw new Exception("Không tìm thấy thành phố phù hợp");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static IEnumerable<CityResult> SearchCities(JArray countriesArray, string searchTerm)
        {
            // Tìm kiếm thành phố dựa trên chuỗi đầu vào với phương thức StartsWith cho phép so sánh không phân biệt chữ hoa chữ thường và hỗ trợ tiếng Việt
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

                var accountInfo = jsonObj["__DEFAULT_SCOPE__"]["webapp.user-detail"]["userInfo"].ToString();
                return accountInfo;
            }

            return string.Empty;
        }

        public async Task<string> GetVideoTikTokInformation(string url)
        {
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

                var videoInfo = jsonObj["__DEFAULT_SCOPE__"]["webapp.video-detail"]["itemInfo"].ToString();
                return videoInfo;
            }

            return string.Empty;
        }
    }
}
