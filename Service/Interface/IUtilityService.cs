namespace Service.Interface
{
    public interface IUtilityService
    {
        IEnumerable<string> GetCitiesWithCountry(string keyword);
    }
}
