using System.Text.Json.Serialization;

namespace BusinessObjects
{
    public class TagDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public bool? IsPremium { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }

    public class TagRequestDTO
    {
        public string Name { get; set; }

        public bool IsPremium { get; set; }
    }
}
