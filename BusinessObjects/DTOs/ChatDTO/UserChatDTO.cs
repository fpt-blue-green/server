

namespace BusinessObjects
{
	public class UserChatDTO
	{
		public Guid Id { get; set; }
		public Guid SenderId { get; set; }
		public Guid ReceiverId { get; set; }
		public string? Message { get; set; }
		public DateTime? DateSent { get; set; }
		public string? SenderName { get; set; }
		public string? ReceiverName { get; set; }
		public virtual UserMessage Receiver { get; set; } = null!;
		public virtual UserMessage Sender { get; set; } = null!;
	}
}
