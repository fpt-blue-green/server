using System.Text.Json.Serialization;

namespace BusinessObjects
{
    public class TagDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public bool? IsPremium { get; set; }

    }
    public class TagDetailDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public bool? IsPremium { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
