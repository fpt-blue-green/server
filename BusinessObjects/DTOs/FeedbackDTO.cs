namespace BusinessObjects.DTOs
{
    public class FeedbackDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid InfluencerId { get; set; }

        public int? Rating { get; set; }

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public virtual UserDTO User { get; set; } = null!;
    }

    public class FeedbackRequestDTO
    {
        public int? Rating { get; set; }

        public string Content { get; set; } = null!;
    }
}
