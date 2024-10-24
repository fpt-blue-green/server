
using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class FilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? SortBy { get; set; } // Which field wanna sort
        public bool? IsAscending { get; set; } = true;
        public decimal? PriceFrom { get; set; } = 0;
        public decimal? PriceTo { get; set; } = 10000000;
    }
    public class InfluencerFilterDto : FilterDTO
    {
        public List<EGender>? Genders { get; set; }
        public List<Guid>? TagIds { get; set; }
        public List<EPlatform>? Platforms { get; set; }
        public int? RateStart { get; set; }

    }
    public class CampaignFilterDto : FilterDTO
    {
        public List<Guid>? TagIds { get; set; }
    }

    public class JobFilterDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public ERole? From { get; set; }
        public ECampaignStatus? CampaignStatus { get; set; }
        public EJobStatus? JobStatus { get; set; }
    }
}
