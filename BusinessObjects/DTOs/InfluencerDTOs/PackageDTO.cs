namespace BusinessObjects
{
    public class PackageDTO
    {
        public Guid? Id { get; set; }

        public EPlatform Platform { get; set; }

        public EContentType ContentType { get; set; }

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public int Price { get; set; }

        public int? Quantity { get; set; }
    }
}
