using System.IdentityModel.Tokens.Jwt;
using BusinessObjects.DTOs;
using BusinessObjects.Enum;
using Service.Interface.HelperService;
using System.Security.Claims;
using System.Text.Json;
using static BusinessObjects.Enum.AuthEnumContainer;

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

            // Kiểm tra xem endpoint hiện tại có được đánh dấu miễn trừ không
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
                await WriteUnauthorizedResponse(context, "Token bị thiếu hoặc không hợp lệ.");
                return;
            }

            // Xóa "Bearer " khỏi token
            token = token.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                await WriteUnauthorizedResponse(context, "Token bị thiếu.");
                return;
            }

            try
            {
                // Đọc token và kiểm tra thời gian hết hạn
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                var expirationTime = jwtToken?.ValidTo;

                // Kiểm tra nếu token còn dưới 5 giây
                if (expirationTime.HasValue && expirationTime.Value - DateTime.UtcNow < TimeSpan.FromSeconds(5))
                {
                    await WriteUnauthorizedResponse(context, "Token đã hết hạn.");
                    return;
                }

                // Xác thực token
                var userName = await _securityService.ValidateJwtToken(token);
                if (userName == null)
                {
                    await WriteUnauthorizedResponse(context, "Token không hợp lệ.");
                    return;
                }

                // Thiết lập thông tin người dùng vào context
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, userName)
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
                        await WriteForbiddenResponse(context, "Tài khoản không có quyền truy cập.");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                await WriteErrorResponse(context, "Đã xảy ra lỗi khi xác thực token.");
                return;
            }

            // Cho phép request tiếp tục đến controller nếu token hợp lệ
            await _next(context);
        }

        private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
        {
            var response = new ApiResponse<string>
            {
                StatusCode = EHttpStatusCode.Unauthorized,
                Message = message,
                Data = null
            };

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static async Task WriteForbiddenResponse(HttpContext context, string message)
        {
            var response = new ApiResponse<string>
            {
                StatusCode = EHttpStatusCode.Forbidden,
                Message = message,
                Data = null
            };

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static async Task WriteErrorResponse(HttpContext context, string message)
        {
            var response = new ApiResponse<string>
            {
                StatusCode = EHttpStatusCode.InternalServerError,
                Message = message,
                Data = null
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
