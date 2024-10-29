using BusinessObjects;

namespace Service
{
    public interface IAdminStatisticService
    {
        Task<Dictionary<string, int>> GetLoginCountsByTimeFrame(int year, ETimeFrame timeFrame);
    }
}
