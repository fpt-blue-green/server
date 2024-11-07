

namespace BusinessObjects
{
    public class MessageResDTO
    {
        public Guid SenderId { get; set; }

        public Guid? ReceiverId { get; set; }

        public Guid? CampaignChatId { get; set; }

        public string Content { get; set; } = null!;

    }
}
