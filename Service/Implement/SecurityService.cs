using Microsoft.IdentityModel.Tokens;
using Service.Domain;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Implement
{
    public class SecurityService : ISecurityService
    {
        private static ConfigManager _configManager = new ConfigManager();
        public async Task<string> GenerateJwtToken(string data, bool isAdmin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("JWTKey");
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, data)
                };

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("JWTKey");

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Bỏ qua kiểm tra Issuer
                    ValidateAudience = false, // Bỏ qua kiểm tra Audience
                    ValidateLifetime = true, // Kiểm tra thời gian sống của token
                    ValidateIssuerSigningKey = true, // Kiểm tra chữ ký của token
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return principal.Identity?.Name!;
            }
            catch (Exception ex)
            {
                return null!;
            }
        }
    }
}
