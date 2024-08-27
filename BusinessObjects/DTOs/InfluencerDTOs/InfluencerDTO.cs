using BusinessObjects.DTOs.InfluencerDTO;

namespace BusinessObjects.DTOs.InfluencerDTOs
{
    public class InfluencerDTO
    {
        public Guid Id { get; set; }

        /* public Guid UserId { get; set; }*/

        public string FullName { get; set; } = null!;

        public string NickName { get; set; } = null!;

        public int? Gender { get; set; }

        public string? Bio { get; set; }

        public string Phone { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public decimal? RateAverage { get; set; }
        public decimal? AveragePrice { get; set; }

        public virtual ICollection<ChannelDTO> Channels { get; set; } = new List<ChannelDTO>();
        public virtual ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public virtual ICollection<PackageDTO> Packages { get; set; } = new List<PackageDTO>();
    }
}
