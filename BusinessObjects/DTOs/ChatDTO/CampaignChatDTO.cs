

using BusinessObjects.Models;

namespace BusinessObjects
{
	public class CampaignChatDTO
	{
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string RoomName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<ChatMemberDTO> Members { get; set; } = new List<ChatMemberDTO>();
    }
    public class CampaignChatResDTO
    {
        public Guid CampaignId { get; set; }
    }
}
