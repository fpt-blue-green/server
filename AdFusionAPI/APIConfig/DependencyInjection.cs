using Service;

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
            services.AddScoped<IBrandService, BrandService>();
			services.AddScoped<IBrandService, BrandService>();
			services.AddScoped<ICampaignService, CampaignService>();
			services.AddScoped<ICampaignContentService, CampaignContentService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IOfferService, OfferService>();

        }
	}
}
