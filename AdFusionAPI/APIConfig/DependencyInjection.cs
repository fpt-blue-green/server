﻿using Service.Domain;
using Service.Implement;
using Service.Interface;
using System.ComponentModel.Design;

namespace AdFusionAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenService, AuthenService>();
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
