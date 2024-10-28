
using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class FilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? Search { get; set; }
        public string? SortBy { get; set; } // Which field wanna sort
        public bool? IsAscending { get; set; } = true;
        public decimal? PriceFrom { get; set; } = 0;
        public decimal? PriceTo { get; set; } = 10000000;
    }
    public class InfluencerFilterDTO : FilterDTO
    {
        public List<EGender>? Genders { get; set; }
        public List<Guid>? TagIds { get; set; }
        public List<EPlatform>? Platforms { get; set; }
        public int? RateStart { get; set; }

    }
    public class CampaignFilterDTO : FilterDTO
    {
        public List<Guid>? TagIds { get; set; }
    }

    public class JobFilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public ERole? From { get; set; }
        public ECampaignStatus[]? CampaignStatuses { get; set; }
        public EJobStatus[]? JobStatuses { get; set; }
    }

    public class ReportFilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public EReportStatus[]? ReportStatus { get; set; }
        public EReportReason[]? ReportReasons { get; set; }
    }

    public class TagFilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public bool? IsPremium { get; set; }
    }

    public class PaymentWithDrawFilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public EPaymentStatus[] PaymentStatus { get; set; }
        public EPaymentType[] PaymentType { get; set; }
    }
}
