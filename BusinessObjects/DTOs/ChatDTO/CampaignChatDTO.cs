

using BusinessObjects.Models;

namespace BusinessObjects
{
	public class CampaignChatDTO
	{
		public Guid Id { get; set; }
		public Guid CampaignId { get; set; }
		public Guid? SenderId { get; set; }
		public string? RoomName { get; set; }
		public string Message { get; set; }
		public DateTime SendTime { get; set; }
		public UserMessage? Sender { get; set; }
	}
	public class UserMessage
	{
		public Guid Id { get; set; }
		public string Email { get; set; } = null!;
		public string? DisplayName { get; set; }
		public string? Avatar { get; set; }
	}
}
