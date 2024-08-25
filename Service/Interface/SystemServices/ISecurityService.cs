namespace Service.Interface.HelperService
{
    public interface ISecurityService
    {
        Task<string> GenerateAuthenToken(string data, bool isAdmin);
        Task<string> GenerateRefreshToken(string data, bool isAdmin);
        Task<string> ValidateJwtToken(string token);
        string ComputeSha256Hash(string rawData);

    }
}
