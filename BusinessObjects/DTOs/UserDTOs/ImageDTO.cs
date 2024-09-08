namespace BusinessObjects
{
    public class ImageDTO
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = null!;

        public string? Description { get; set; }
    }
}
