using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class OfferDTO
    {
        public int? Platform { get; set; }

        public int? ContentType { get; set; }

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public int? Price { get; set; }

        public EOfferStatus Status { get; set; }

        public ERole From { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? Quantity { get; set; }
    }

    public class OfferCreateRequestDTO
    {
        public JobDTO Job { get; set; }
        public OfferDTO Offer { get; set; }
    }
}
