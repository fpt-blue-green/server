﻿using Service;
using Service.Implement;

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
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ConfigManager>();
            services.AddScoped<IPackageService, PackageService>();
        }
    }
}
