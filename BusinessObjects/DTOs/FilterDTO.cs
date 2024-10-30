
using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class FilterListResponse<T>
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }

    #region Filter
    public class FilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? Search { get; set; }
        public string? SortBy { get; set; } // Which field wanna sort
        public bool? IsAscending { get; set; } = true;
    }

    public class InfluencerFilterDTO : FilterDTO
    {
        public List<EGender>? Genders { get; set; }
        public List<Guid>? TagIds { get; set; }
        public List<EPlatform>? Platforms { get; set; }
        public int? RateStart { get; set; }
        public decimal? PriceFrom { get; set; } = 0;
        public decimal? PriceTo { get; set; } = 10000000;

    }

    public class CampaignFilterDTO : FilterDTO
    {
        public List<Guid>? TagIds { get; set; }
        public decimal? PriceFrom { get; set; } = 0;
        public decimal? PriceTo { get; set; } = 10000000;
    }

    public class JobFilterDTO : FilterDTO
    {
        public ERole? From { get; set; }
        public ECampaignStatus[]? CampaignStatuses { get; set; }
        public EJobStatus[]? JobStatuses { get; set; }
    }

    public class ReportFilterDTO : FilterDTO
    {
        public EReportStatus[]? ReportStatus { get; set; }
        public EReportReason[]? ReportReasons { get; set; }
    }

    public class TagFilterDTO : FilterDTO
    {
        public bool? IsPremium { get; set; }
    }

    public class PaymentWithDrawFilterDTO : FilterDTO
    {
        public EPaymentStatus[]? PaymentStatus { get; set; }
        public EPaymentType[]? PaymentType { get; set; }
    }

    public class BrandCampaignFilterDTO : FilterDTO
    {
        public ECampaignStatus[]? CampaignStatus { get; set; }
    }
    public class UserFilterDTO : FilterDTO
    {
        public ERole[]? Roles { get; set; }
        public EAccountProvider[]? Providers { get; set; }
    }
    public class InfluencerJobFilterDTO : FilterDTO
    {
        public EJobStatus[]? JobStatuses { get; set; }
        public EOfferStatus[]? OfferStatuses { get; set; }
    }
    #endregion
}
