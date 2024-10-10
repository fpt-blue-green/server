using BusinessObjects.Models;

namespace BusinessObjects
{
    public class FavoriteDTO
    {
        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public Guid InfluencerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual InfluencerDTO Influencer { get; set; } = null!;
    }
}
