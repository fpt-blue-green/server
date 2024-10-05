using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class JobDTO
    {
        public Guid? InfluencerId { get; set; }

        public Guid CampaignId { get; set; }

        public EJobStatus Status { get; set; }

        public string? Link { get; set; } = null!;
    }

    public class JobLinkDTO
    {
        public string Link { get; set; }
    }
}
