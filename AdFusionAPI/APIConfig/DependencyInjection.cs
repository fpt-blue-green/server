using Service;

namespace AdFusionAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<CampaignMeetingRoomService, CampaignMeetingRoomService>();
            services.AddScoped<ICloudinaryStorageService, CloudinaryStorageService>();
            services.AddScoped<ICampaignContentService, CampaignContentService>();
            services.AddScoped<IAdminStatisticService, AdminStatisticService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IAdminActionService, AdminActionService>();
            services.AddScoped<IBannedUserService, BannedUserService>();
            services.AddScoped<IInfluencerService, InfluencerService>();
            services.AddScoped<IJobDetailService, JobDetailService>();
            services.AddScoped<IFeedBackService, FeedBackService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IPaymentService, PaymentService>();
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


            services.AddSingleton<ISecurityService, SecurityService>();
        }
    }
}
