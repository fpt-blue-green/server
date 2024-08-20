using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service.Interface.SystemServices;
using System.Text;

namespace AdFusionAPI
{
    public static class JwtConfiguration
    {
        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var systemSetting = serviceProvider.GetRequiredService<ISystemSettingService>();
            var jwtSettings = systemSetting.GetJWTSystemSetting();
            var key = jwtSettings.Result.KeyValue ?? null;


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
