﻿
namespace BusinessObjects
{
	public class CampaignContentResDto
	{
		public EPlatform Platform { get; set; }
		public EContentType ContentType { get; set; }
		public int Quantity { get; set; }
		public string Content { get; set; } = null!;
	}
}
