using BusinessObjects;
using BusinessObjects.Models;
using Pgvector;
using Repositories;
using System.Globalization;
using System.Text;

namespace Service.Helper
{

  public class EmbeddingUpdater
  {
    private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
    private static readonly OpenAIEmbeddingHelper _openAIEmbeddingHelper = new OpenAIEmbeddingHelper();

    private string CreateInfluencerPrompt(Influencer influencer)
    {
      StringBuilder sb = new StringBuilder();

      // Thêm thông tin chính
      sb.Append($"{influencer.FullName} là một nhà sáng tạo nội dung ");
      switch ((EGender)influencer.Gender)
      {
        case EGender.Male:
          sb.Append("nam ");
          break;
        case EGender.Female:
          sb.Append("nữ ");
          break;
        default:
          break;
      }
      sb.Append($"nổi bật chuyên về {influencer.Summarise}. ");
      sb.Append($"Họ tự mô tả mình là: \"{influencer.Description}\". ");

      if (!string.IsNullOrWhiteSpace(influencer.Address))
      {
        sb.Append($"Địa chỉ chính hiện tại: {influencer.Address}. ");
      }

      if (influencer.Tags?.Any() == true)
      {
        sb.Append("Các lĩnh vực nội dung chính bao gồm: ");
        var tagStr = string.Join(", ", influencer.Tags.Select(tag => tag.Name));
        sb.Append($"{tagStr}. ");
      }

      if (influencer.Channels?.Any() == true)
      {
        sb.Append("Hoạt động trên các kênh: ");
        foreach (var channel in influencer.Channels)
        {
          var platform = "";
          switch ((EPlatform)channel.Platform)
          {
            case EPlatform.Tiktok:
              platform = "TikTok";
              break;
            case EPlatform.Instagram:
              platform = "Instagram";
              break;
            case EPlatform.Youtube:
              platform = "YouTube";
              break;
            default:
              break;
          }
          sb.Append($"{platform} với {channel.FollowersCount:N0} người theo dõi, ");
        }
        sb.Length -= 2; // Loại bỏ dấu phẩy cuối cùng
        sb.Append(". ");
      }
      if (influencer.AveragePrice.HasValue)
      {
        sb.Append($"Giá trung bình cho mỗi công việc là {influencer.AveragePrice.Value.ToString("C0", new CultureInfo("vi-VN"))}. ");
      }

      return sb.ToString();
    }

    public async Task UpdateInfluencerEmbedding(Guid influencerId)
    {
      var influencer = await _influencerRepository.GetById(influencerId);

      if (influencer != null && influencer.IsPublish)
      {
        var prompt = CreateInfluencerPrompt(influencer);
        var embedding = await _openAIEmbeddingHelper.GetEmbeddingAsync(prompt);
        influencer.Embedding = new Vector(embedding);
        await _influencerRepository.Update(influencer);
      }
    }
  }
}
