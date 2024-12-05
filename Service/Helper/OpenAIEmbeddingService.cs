using Service.Implement.UtilityServices;
using Service.Interface.UtilityServices;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class OpenAIEmbeddingService
{
    private readonly HttpClient _httpClient;
    private static readonly IEnvService _envService = new EnvService();
    private const string ApiUrl = "https://api.openai.com/v1/embeddings";
    private string ApiKey = "YOUR_API_KEY"; // Thay bằng API Key của bạn

    public OpenAIEmbeddingService()
    {
        ApiKey = _envService.GetEnv("OPENAI_KEY");
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
    }

    public async Task<float[]> GetEmbeddingAsync(string prompt)
    {
        var payload = new
        {
            input = prompt,
            model = "text-embedding-ada-002"
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(ApiUrl, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<EmbeddingResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (responseData != null)
        {
            return responseData.Data[0].Embedding;
        }
        return null;
    }
}

public class EmbeddingResponse
{
    public string Object { get; set; }
    public List<EmbeddingData> Data { get; set; }
    public string Model { get; set; }
    public Usage Usage { get; set; }
}

public class EmbeddingData
{
    public string Object { get; set; }
    public int Index { get; set; }
    public float[] Embedding { get; set; }
}

public class Usage
{
    public int Prompt_Tokens { get; set; }
    public int Total_Tokens { get; set; }
}