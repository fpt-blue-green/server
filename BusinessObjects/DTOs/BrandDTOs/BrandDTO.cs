using BusinessObjects;

namespace BusinessObjects
{
    public class BrandDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Banner { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual ICollection<CampaignDTO> Campaigns { get; set; } = new List<CampaignDTO>();


    }
}
