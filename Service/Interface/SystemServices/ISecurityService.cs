﻿namespace Service
{
    public interface ISecurityService
    {
        Task<string> GenerateAuthenToken(string data, int expire = 15);
        Task<string> GenerateRefreshToken(string data);
        Task<string> ValidateJwtToken(string token);
        string ComputeSha256Hash(string rawData);

    }
}
