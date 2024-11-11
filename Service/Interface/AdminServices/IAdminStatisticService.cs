using BusinessObjects;

namespace Service
{
    public interface IAdminStatisticService
    {
        Task<Dictionary<string, int>> GetLoginCountsByTimeFrame(int year, ETimeFrame timeFrame);
        Task<List<int>> GetAvailableYearInActiveUser();
        Task<Dictionary<string, int>> GetRoleData();
        Task<List<MonthlyMetricsTrendDTO>> GetMonthlyMetricsTrend();
        Task<Dictionary<string, int>> GetJobStatusData();
        Task<List<TopFiveStatisticDTO>> GetTopFiveInfluencerUser();
        Task<List<TopFiveStatisticDTO>> GetTopFiveBrandUser();
    }
}
