using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.Enum;

namespace BusinessObjects.DTOs.InfluencerDTOs
{
    public class InfluencerRequestDTO
    {
        public string FullName { get; set; } = null!;

        public string Slug { get; set; }

        public string Summarise { get; set; }

        public string Description { get; set; }

        public EGender Gender { get; set; }

        public string Phone { get; set; } = null!;

        public string Address { get; set; }

    }
}
