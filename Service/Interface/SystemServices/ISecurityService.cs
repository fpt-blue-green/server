namespace Service.Interface.HelperService
{
    public interface ISecurityService
    {
        Task<string> GenerateJwtToken(string data, bool isAdmin);
        Task<string> ValidateJwtToken(string token);
        string ComputeSha256Hash(string rawData);

    }
}
