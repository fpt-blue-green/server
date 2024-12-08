using Service.Implement.UtilityServices;
using Service.Interface.UtilityServices;
using System.Text;
using System.Text.Json;

public class OpenAIEmbeddingHelper
{
    private readonly HttpClient _httpClient;
    private static readonly IEnvService _envService = new EnvService();
    private const string EmbeddingUrl = "https://api.openai.com/v1/embeddings";
    private const string ChatCompletionUrl = "https://api.openai.com/v1/chat/completions";
    private string ApiKey = "YOUR_API_KEY"; // Thay bằng API Key của bạn

    public OpenAIEmbeddingHelper()
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

        var response = await _httpClient.PostAsync(EmbeddingUrl, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<EmbeddingResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (responseData != null)
        {
            return responseData.Data[0].Embedding;
        }
        return null;
    }

    public async Task<string> NormalizePromptAsync(string userInput)
    {
        var systemPrompt = @"
Bạn là một trợ lý chuyên chuẩn hóa dữ liệu tìm kiếm. Hãy chuyển đổi các câu hỏi hoặc mô tả từ người dùng thành một định dạng chuẩn để phục vụ tìm kiếm và gợi ý nội dung. 

Định dạng chuẩn:
- Tên: [Tên của influencer nếu có].
- Giới tính: [Nam/Nữ].
- Lĩnh vực: [Lĩnh vực chính hoặc tóm tắt].
- Mô tả: [Mô tả chi tiết].
- Địa chỉ: [Địa chỉ nếu có].
- Lĩnh vực chính: [Các lĩnh vực/tags nếu có].
- Kênh hoạt động: [Các nền tảng như Instagram, TikTok,... và số người theo dõi].
- Giá trung bình: [Giá nếu có]

Hãy đảm bảo trả về đúng định dạng JSON. Nếu không có đủ thông tin, hãy để trống các trường đó.";

        var payload = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = userInput }
        },
            temperature = 0.7
        };


        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(ChatCompletionUrl, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var chatResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return chatResponse.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
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

public class ChatCompletionResponse
{
    public List<Choice> Choices { get; set; }
}

public class Choice
{
    public MessageAI Message { get; set; }
}

public class MessageAI
{
    public string Content { get; set; }
    public string Role { get; set; }
}