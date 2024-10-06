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
        public virtual BrandDTO Brand { get; set; } = new BrandDTO();
		public virtual ICollection<CampaignContentResDto> Contents { get; set; } = new List<CampaignContentResDto>();
		public virtual ICollection<CampaignImageDto> Images { get; set; } = new List<CampaignImageDto>();

	}
}
