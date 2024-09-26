

namespace BusinessObjects
{
	public class CampaignContentDto
	{
		public Guid? Id { get; set; }
		public EPlatform Platform { get; set; }
		public EContentType ContentType { get; set; }
		public int Quantity { get; set; }
		public string Content { get; set; } = null!;
	}
}
