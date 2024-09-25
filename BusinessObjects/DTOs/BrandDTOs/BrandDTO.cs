using BusinessObjects;

namespace BusinessObjects
{
    public class BrandDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; } = null!;
        public string CoverImg { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public string WebsiteUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string TiktokUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual ICollection<CampaignDTO> Campaigns { get; set; } = new List<CampaignDTO>();


    }
}
