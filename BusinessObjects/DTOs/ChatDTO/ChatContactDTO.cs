
namespace BusinessObjects
{
    public class ChatContactDTO
    {
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
        public Guid? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public DateTime? LastInteractionTime { get; set; }
        public string? LastMessage { get; set; }

    }

    public class ChatPartnerDTO
    {
        public Guid? ChatId { get; set; }
        public string ChatName { get; set; }
        public string? ChatImage { get; set; }
        public UserDTO Sender { get; set; }
        public string LastMessage { get; set; }
        public DateTime SentAt { get; set; }
        public bool isCampaign { get; set; }
    }
}
