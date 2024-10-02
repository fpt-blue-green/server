
namespace BusinessObjects
{
    public class FilterDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public List<Guid>? TagIds { get; set; }
        public string? Search { get; set; }
        public List<EGender>? Genders { get; set; }
        public List<EPlatform>? Platforms { get; set; }
        public string? SortBy { get; set; } // Which field wanna sort
        public bool? IsAscending { get; set; } = true;
        public int? RateStart { get; set; }
        public decimal? PriceFrom { get; set; } = 0;
        public decimal? PriceTo { get; set; } = 10000000;
    }
}
