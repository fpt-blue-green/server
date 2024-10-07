

namespace BusinessObjects
{
	public class CampaignContentDto
	{
		public Guid? Id { get; set; }
		public EPlatform Platform { get; set; }
		public EContentType ContentType { get; set; }
		public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public int TargetReaction { get; set; }
        public string Description { get; set; } = null!;
	}
}
