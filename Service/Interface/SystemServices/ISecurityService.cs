namespace Service
{
    public interface ISecurityService
    {
        Task<string> GenerateAuthenToken(string data, int expire = 15);
        Task<string> GenerateRefreshToken(string data);
        Task<string> ValidateJwtAuthenToken(string token);
        Task<string> DecryptJWTAccessToken(string token);
        string ComputeSha256Hash(string rawData);
        Task<string> ValidateJwtEmailToken(string token);
    }
}
