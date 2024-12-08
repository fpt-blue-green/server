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
        private static readonly IEmbeddingRepository _embeddingRepository = new EmbeddingRepository();

        private string CreateInfluencerPrompt(Influencer influencer)
        {

            var prompt = @"  - Tên: {ten}.
                        - Giới tính: {gioitinh}.
                        - Lĩnh vực: {tomtat}.
                        - Mô tả: {mota}.
                        - Địa chỉ: {diachi}.
                        - Lĩnh vực chính: {the}.
                        - Kênh hoạt động: {kenh}.
                        - Giá trung bình: {gia}
                        ";
            StringBuilder sb = new StringBuilder(prompt);

            sb.Replace("{ten}", influencer.FullName);


            switch ((EGender)influencer.Gender)
            {
                case EGender.Male:
                    sb.Replace("{gioitinh}", "Nam");
                    break;
                case EGender.Female:
                    sb.Replace("{gioitinh}", "Nữ");
                    break;
                default:
                    sb.Replace("{gioitinh}", "Khác");
                    break;
            }
            sb.Replace("{tomtat}", influencer.Summarise);
            sb.Replace("{mota}", influencer.Description);

            if (!string.IsNullOrWhiteSpace(influencer.Address))
            {
                sb.Replace("{diachi}", influencer.Address);
            }

            if (influencer.Tags?.Any() == true)
            {
                var tagStr = string.Join(", ", influencer.Tags.Select(tag => tag.Name));
                sb.Replace("{the}", tagStr);

            }

            if (influencer.Channels?.Any() == true)
            {
                var channelStr = "";
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
                    channelStr += ($"{platform} ({channel.FollowersCount:N0} người theo dõi), ");
                }
                sb.Replace("{kenh}", channelStr);

            }

            if (influencer.AveragePrice.HasValue)
            {
                sb.Replace("{gia}", influencer.AveragePrice.Value.ToString("C0", new CultureInfo("vi-VN")));
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
                var embeddingInflue = await _embeddingRepository.GetEmbeddingByInfluencerId(influencerId);

                if (embeddingInflue != null)
                {
                    //update new
                    embeddingInflue.EmbeddingValue = new Vector(embedding);
                    await _embeddingRepository.Update(embeddingInflue);
                }
                else
                {
                    var newEmbedding = new Embedding
                    {
                        InfluencerId = influencerId,
                        CampaignId = null,
                        EmbeddingValue = new Vector(embedding),
                    };
                    await _embeddingRepository.Create(newEmbedding);
                }
            }
        }
    }
}
