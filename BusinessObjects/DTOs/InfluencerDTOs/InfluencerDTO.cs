
namespace BusinessObjects
{
    public class InfluencerDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Slug {  get; set; }

        public string Avatar {  get; set; } = null!;
      
        public int? Gender { get; set; }
      
        public string Summarise { get; set; }
      
        public string? Description { get; set; }
      
        public string Address { get; set; } = null!;

        public string? Phone { get; set; } = null!;

        public bool? IsPublish { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public decimal? RateAverage { get; set; }
        public decimal? AveragePrice { get; set; }

        public virtual ICollection<ChannelDTO> Channels { get; set; } = new List<ChannelDTO>();
        public virtual ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public virtual ICollection<PackageDTO> Packages { get; set; } = new List<PackageDTO>();
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();
    }
}
