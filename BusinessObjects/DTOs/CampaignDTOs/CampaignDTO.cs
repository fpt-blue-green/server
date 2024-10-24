using BusinessObjects.Models;

namespace BusinessObjects
{
    public class CampaignDTO
    {
		public Guid? Id { get; set; }
		public string Name { get; set; } = null!;
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public decimal? Budget { get; set; }
        public ECampaignStatus Status { get; set; }
        public virtual BrandDTO Brand { get; set; } = new BrandDTO();
		public virtual ICollection<CampaignContentResDto> Contents { get; set; } = new List<CampaignContentResDto>();
		public virtual ICollection<CampaignImageDto> Images { get; set; } = new List<CampaignImageDto>();
		public virtual ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public virtual ICollection<CampaignMeetingRoomDTO> CampaignMeetingRooms { get; set; } = new List<CampaignMeetingRoomDTO>();

    }
}
