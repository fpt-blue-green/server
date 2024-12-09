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
        private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
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
                        - Giá trung bình: {gia}.
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
                    channelStr += $"{platform} ({channel.FollowersCount:N0} người theo dõi), ";
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

        private string CreateCampaignPrompt(Campaign campaign)
        {
            var prompt = @"  - Tên và lĩnh vực: {ten}.
                        - Mô tả: {mota}.
                        - Lĩnh vực chính: {the}.
                        - Kênh hoạt động: {kenh}.
                        - Giá trung bình: {gia}.
                        ";
            StringBuilder sb = new StringBuilder(prompt);

            sb.Replace("{ten}", campaign.Title);

            sb.Replace("{mota}", campaign.Description);

            if (campaign.Tags?.Any() == true)
            {
                var tagStr = string.Join(", ", campaign.Tags.Select(tag => tag.Name));
                sb.Replace("{the}", tagStr);
            }

            if (campaign.CampaignContents?.Any() == true)
            {
                var price = campaign.CampaignContents.Average(c => c.Price);
                sb.Replace("{gia}", price.Value.ToString("C0", new CultureInfo("vi-VN")));

                var channelStr = "";
                foreach (var platform in campaign.CampaignContents.Select(channel => channel.Platform).Distinct())
                {
                    var platformStr = "";
                    switch ((EPlatform)platform)
                    {
                        case EPlatform.Tiktok:
                            platformStr = "TikTok";
                            break;
                        case EPlatform.Instagram:
                            platformStr = "Instagram";
                            break;
                        case EPlatform.Youtube:
                            platformStr = "YouTube";
                            break;
                        default:
                            break;
                    }
                    channelStr += $"{platformStr}, ";
                }
                sb.Replace("{kenh}", channelStr);
            }

            return sb.ToString();
        }

        public async Task UpdateCampaignEmbedding(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetById(campaignId);

            if (campaign != null && (ECampaignStatus)campaign.Status == ECampaignStatus.Published)
            {
                var prompt = CreateCampaignPrompt(campaign);
                Console.WriteLine(prompt);
                var embedding = await _openAIEmbeddingHelper.GetEmbeddingAsync(prompt);
                var embeddingCam = await _embeddingRepository.GetEmbeddingByCampaignId(campaignId);

                if (embeddingCam != null)
                {
                    //update new
                    embeddingCam.EmbeddingValue = new Vector(embedding);
                    await _embeddingRepository.Update(embeddingCam);
                }
                else
                {
                    var newEmbedding = new Embedding
                    {
                        InfluencerId = null,
                        CampaignId = campaignId,
                        EmbeddingValue = new Vector(embedding),
                    };
                    await _embeddingRepository.Create(newEmbedding);
                }
            }
        }
    }
}
