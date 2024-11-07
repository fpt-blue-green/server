

namespace BusinessObjects
{
    public class ChatMemberDTO
    {
        public Guid CampaignChatId { get; set; }

        public Guid UserId { get; set; }

        public DateTime? JoinAt { get; set; }
        public virtual UserMessage User { get; set; } = null!;

    }
    public class ChatMemberResDTO
    {
        public Guid CampaignChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
