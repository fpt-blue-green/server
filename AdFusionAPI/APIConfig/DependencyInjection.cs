using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Domain;
using Service.Implement;
using Service.Interface;

namespace AdFusionAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services)
        {
			services.AddScoped<ISecurityService, SecurityService>();
            services.AddSingleton<IUtilityService, UtilityService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
			services.AddSingleton<IInfluencerService, InfluencerService>();
			services.AddSingleton<IFeedBackService, FeedBackService>();
			services.AddSingleton<IUtilityService, UtilityService>();
            services.AddScoped<ConfigManager>();
        }
    }
}
