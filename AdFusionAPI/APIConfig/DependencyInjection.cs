using Service;

namespace AdFusionAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<ICloudinaryStorageService, CloudinaryStorageService>();
            services.AddScoped<ICampaignContentService, CampaignContentService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IAdminActionService, AdminActionService>();
            services.AddScoped<IBannedUserService, BannedUserService>();
            services.AddScoped<IInfluencerService, InfluencerService>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddScoped<IJobDetailService, JobDetailService>();
            services.AddScoped<CampaignMeetingRoomService, CampaignMeetingRoomService>();
            services.AddScoped<IFeedBackService, FeedBackService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<ConfigManager>();

        }
    }
}
