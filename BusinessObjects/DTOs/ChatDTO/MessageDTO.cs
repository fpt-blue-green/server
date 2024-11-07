

using BusinessObjects.Models;

namespace BusinessObjects
{
	public class MessageDTO
	{
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid? ReceiverId { get; set; }

        public Guid? CampaignChatId { get; set; }

        public string Content { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public virtual UserMessage? Receiver { get; set; } = null!;
		public virtual UserMessage Sender { get; set; } = null!;
	}
/*    public class CampaignChatMessageDTO
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; } = null!;
    }*/
    public class UserMessage
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? Image { get; set; }
    }
}
