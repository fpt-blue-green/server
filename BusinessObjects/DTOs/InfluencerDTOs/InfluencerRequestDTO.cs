using BusinessObjects.DTOs.InfluencerDTO;

namespace BusinessObjects.DTOs.InfluencerDTOs
{
    public class InfluencerRequestDTO
    {
        public string FullName { get; set; } = null!;

        public string NickName { get; set; } = null!;

        public int? Gender { get; set; }

        public string? Bio { get; set; }

        public string Phone { get; set; } = null!;

        public decimal AveragePrice { get; set; }

        public virtual ICollection<TagRequestDTO> InfluencerTags { get; set; } = new List<TagRequestDTO>();

    }
}
