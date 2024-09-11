
namespace BusinessObjects
{
    public class InfluencerFilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public List<Guid>? TagIds { get; set; }
        public string? SearchString { get; set; }
        public List<EGender>? Genders { get; set; }
        public bool? IsSortAcsPrice { get; set; }
        public bool? IsSortDesPrice { get; set; }
        public bool? IsSortRate { get; set; } = true; //default load page
        public List<int>? RateStart { get; set; }
        public decimal? PriceFrom { get; set; } = 0;
        public decimal? PriceTo { get; set; } = 100000;
    }
}
