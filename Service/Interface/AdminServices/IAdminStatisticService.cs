﻿using BusinessObjects;
using BusinessObjects.DTOs;

namespace Service
{
    public interface IAdminStatisticService
    {
        Task<List<CommomPieChartDTO>> GetLoginCountsByTimeFrame(int year, ETimeFrame timeFrame);
        Task<List<CommomPieChartDTO>> GetRevenuesByTimeFrame(int year, ETimeFrame timeFrame);
        List<int> GetAvailableYearInSystem();
        Task<List<CommomPieChartDTO>> GetRoleData();
        Task<List<MonthlyMetricsTrendDTO>> GetMonthlyMetricsTrend();
        Task<List<CommomPieChartDTO>> GetJobStatusData();
        Task<List<TopFiveStatisticDTO>> GetTopFiveInfluencerUser();
        Task<List<TopFiveStatisticDTO>> GetTopFiveBrandUser();
    }
}
