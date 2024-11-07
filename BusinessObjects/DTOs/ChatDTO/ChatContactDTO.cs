
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
    }
}
