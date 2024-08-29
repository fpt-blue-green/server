namespace Service.Interface.HelperService
{
    public interface ISecurityService
    {
        Task<string> GenerateAuthenToken(string data);
        Task<string> GenerateRefreshToken(string data);
        Task<string> ValidateJwtToken(string token);
        string ComputeSha256Hash(string rawData);

    }
}
