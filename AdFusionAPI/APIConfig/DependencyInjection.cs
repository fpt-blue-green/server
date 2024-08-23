using Service.Domain;
using Service.Implement;
using Service.Implement.SystemService;
using Service.Implement.UtilityServices;
using Service.Interface;
using Service.Interface.HelperService;
using Service.Interface.SystemServices;
using Service.Interface.UtilityServices;

namespace AdFusionAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<ICloudinaryStorageService, CloudinaryStorageService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IInfluencerService, InfluencerService>();
            services.AddScoped<IFeedBackService, FeedBackService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ConfigManager>();
        }
    }
}
