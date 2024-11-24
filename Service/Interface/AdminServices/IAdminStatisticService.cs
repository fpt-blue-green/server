using BusinessObjects;
using BusinessObjects.DTOs;

namespace Service
{
    public interface IAdminStatisticService
    {
        Task<Dictionary<string, int>> GetLoginCountsByTimeFrame(int year, ETimeFrame timeFrame);
        Task<List<int>> GetAvailableYearInActiveUser();
        Task<List<CommomPieChartDTO>> GetRoleData();
        Task<List<MonthlyMetricsTrendDTO>> GetMonthlyMetricsTrend();
        Task<List<CommomPieChartDTO>> GetJobStatusData();
        Task<List<TopFiveStatisticDTO>> GetTopFiveInfluencerUser();
        Task<List<TopFiveStatisticDTO>> GetTopFiveBrandUser();
    }
}
