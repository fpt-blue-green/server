using Service.Domain;
using Service.Implement;
using Service.Implement.SystemService;
using Service.Implement.UtilityServices;
using Service.Interface;
using Service.Interface.HelperService;
using Service.Interface.SystemServices;
using Service.Interface.UtilityServices;
using System.ComponentModel.Design;

namespace AdFusionAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IInfluencerService, InfluencerService>();
            services.AddScoped<IFeedBackService, FeedBackService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<ConfigManager>();
			services.AddScoped<ITagService, TagService>();

		}
	}
}
