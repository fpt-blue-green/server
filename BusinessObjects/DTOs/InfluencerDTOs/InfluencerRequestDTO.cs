
namespace BusinessObjects
{
    public class InfluencerRequestDTO
    {
        public string FullName { get; set; } = null!;

        public string Slug { get; set; }

        public string Summarise { get; set; }

        public string Description { get; set; }

        public EGender Gender { get; set; }

        public string Address { get; set; }

    }
}
