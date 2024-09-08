namespace BusinessObjects
{
    public class PackageDTO
    {
        public Guid? Id { get; set; }

        public int? Platform { get; set; }

        public int? ContentType { get; set; }

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public int? Price { get; set; }

        public int? Quantity { get; set; }
    }
}
