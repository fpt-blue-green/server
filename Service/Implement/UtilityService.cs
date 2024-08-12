using BusinessObjects.ModelsDTO;
using Newtonsoft.Json.Linq;
using Service.Domain;
using Service.Interface;
using System.Globalization;

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
    }
}
