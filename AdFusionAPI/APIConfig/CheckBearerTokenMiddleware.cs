using System.IdentityModel.Tokens.Jwt;
using BusinessObjects.DTOs;
using BusinessObjects.Enum;
using Service.Interface.HelperService;
using System.Security.Claims;
using static BusinessObjects.Enum.AuthEnumContainer;
using Newtonsoft.Json;
using BusinessObjects.DTOs.UserDTOs;

namespace AdFusionAPI.APIConfig
{
    public class CheckBearerTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISecurityService _securityService;

        public CheckBearerTokenMiddleware(RequestDelegate next, ISecurityService securityService)
        {
            _next = next;
            _securityService = securityService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().Split('?')[0];

            // Lấy đánh dấu hiện tại của endpoint
            var endpoint = context.GetEndpoint();
            var noAuthRequired = endpoint?.Metadata.GetMetadata<NoAuthRequiredAttribute>() != null;
            var adminRequired = endpoint?.Metadata.GetMetadata<AdminRequiredAttribute>() != null;

            if (noAuthRequired)
            {
                await _next(context);
                return;
            }

            // Lấy Authorization header
            var token = context.Request.Headers["Authorization"].ToString();

            // Kiểm tra nếu token null hoặc không có "Bearer"
            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException();
            }

            // Xóa "Bearer " khỏi token
            token = token.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException();
            }

            // Đọc token và kiểm tra thời gian hết hạn
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var expirationTime = jwtToken?.ValidTo;

            // Xác thực token
            var tokenData = await _securityService.ValidateJwtToken(token);
            if (tokenData == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Thiết lập thông tin người dùng vào context
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, tokenData)
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            // Kiểm tra quyền admin nếu yêu cầu
            if (adminRequired)
            {
                var isAdmin = context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == ERole.Admin.ToString());
                if (!isAdmin)
                {
                    throw new AccessViolationException();
                }
            }

            context.Items["user"] = JsonConvert.DeserializeObject<UserDTO>(tokenData);

            // Cho phép request tiếp tục đến controller nếu token hợp lệ
            await _next(context);
        }
    }
}
