using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class OfferDTO {
        public Guid? Id { get; set; }

        public EPlatform Platform { get; set; }

        public EContentType ContentType { get; set; }

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public int? Price { get; set; }

        public int TargetReaction { get; set; }

        public EOfferStatus? Status { get; set; }

        public ERole? From { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Quantity { get; set; }
    }

    public class OfferRequestDTO
    {
        public EPlatform Platform { get; set; }

        public EContentType ContentType { get; set; }

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public int? Price { get; set; }

        public int TargetReaction { get; set; }

        public int? Quantity { get; set; }
    }

    public class ReOfferDTO 
    {
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Duration { get; set; }
        public int TargetReaction { get; set; }
        public string? Description { get; set; }
    }

    public class OfferCreateRequestDTO
    {
        public JobRequestDTO Job { get; set; }
        public OfferRequestDTO Offer { get; set; }
    }
}
