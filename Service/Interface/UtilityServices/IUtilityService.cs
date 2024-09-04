namespace Service.Interface.UtilityServices
{
    public interface IUtilityService
    {
        IEnumerable<string> GetCitiesWithCountry(string keyword);

        Task<string> GetChannelProfile(int platform, string channelId);
        Task<string> GetVideoInformation(int platform, string url);
    }
}
