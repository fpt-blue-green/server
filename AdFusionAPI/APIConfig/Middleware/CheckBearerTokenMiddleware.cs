using BusinessObjects;
using Newtonsoft.Json;
using Service;
using static BusinessObjects.AuthEnumContainer;

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
            var authRequired = endpoint?.Metadata.GetMetadata<AuthRequiredAttribute>() != null;
            var adminRequired = endpoint?.Metadata.GetMetadata<AdminRequiredAttribute>() != null;
            var influencerRequired = endpoint?.Metadata.GetMetadata<InfluencerRequiredAttribute>() != null;
            var brandRequired = endpoint?.Metadata.GetMetadata<BrandRequiredAttribute>() != null;

            if (authRequired)
            {
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

                    // Xác thực token
                var tokenData = await _securityService.ValidateJwtAuthenToken(token);

                var user = tokenData == null ? null : JsonConvert.DeserializeObject<UserDTO>(tokenData);

                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                // Kiểm tra quyền admin nếu yêu cầu
                if (adminRequired)
                {
                    //var isAdmin = context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == ERole.Admin.ToString());
                    if (!(user.Role == ERole.Admin))
                    {
                        throw new AccessViolationException();
                    }
                }

                // Kiểm tra quyền influencer nếu yêu cầu
                if (influencerRequired)
                {
                    //var isInfluencer = context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == ERole.Influencer.ToString());
                    if (!(user.Role == ERole.Influencer))
                    {
                        throw new AccessViolationException();
                    }
                }

                // Kiểm tra quyền brand nếu yêu cầu
                if (brandRequired)
                {
                    //var isBrand = context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == ERole.Brand.ToString());
                    if (!(user.Role == ERole.Brand))
                    {
                        throw new AccessViolationException();
                    }
                }

                context.Items["user"] = user;
            }

            await _next(context);
        }
    }
}
