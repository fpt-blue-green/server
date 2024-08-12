namespace Service.Interface
{
    public interface ISecurityService
    {
        Task<string> GenerateJwtToken(string data, bool isAdmin);
        Task<string> ValidateJwtToken(string token);
    }
}
