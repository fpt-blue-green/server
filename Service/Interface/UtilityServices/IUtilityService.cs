namespace Service.Interface.UtilityServices
{
    public interface IUtilityService
    {
        IEnumerable<string> GetCitiesWithCountry(string keyword);

        Task<string> GetTikTokInformation(string url);

        Task<string> GetVideoTikTokInformation(string url);

        Task<string> GetVideoInstagramInformation(string url);

        Task<string> GetInstagramInformation(string url);
    }
}
