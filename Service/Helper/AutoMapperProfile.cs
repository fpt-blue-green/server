using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Influencer
            CreateMap<Influencer, InfluencerDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.InfluencerImages))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar)).ReverseMap();
            CreateMap<InfluencerRequestDTO, InfluencerDTO>().ReverseMap();
            CreateMap<InfluencerRequestDTO, Influencer>().ReverseMap();
            CreateMap<Channel, ChannelDTO>().ReverseMap();
            CreateMap<InfluencerJobDTO, Influencer>().ReverseMap();
            CreateMap<InfluencerImage, ImageDTO>().ReverseMap();
            #endregion
            #region Tag
            CreateMap<Tag, TagDTO>().ReverseMap();
            CreateMap<Tag, TagRequestDTO>().ReverseMap();
            #endregion
            #region User
            CreateMap<User, UserDTO>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
             .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Avatar))
             .ReverseMap()
             .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Image));

            CreateMap<User, UserDetailDTO>().ReverseMap();
            #endregion
            #region Channel
            CreateMap<ChannelStatDTO, Channel>().ReverseMap();
            #endregion
            #region Package
            CreateMap<Package, PackageDTO>().ReverseMap();
            CreateMap<PackageDTO, Package>().ReverseMap();
            CreateMap<Package, PackageDtoRequest>().ReverseMap();
            CreateMap<PackageDtoRequest, Package>().ReverseMap();

            #endregion
            #region SystemSetting
            CreateMap<SystemSetting, SystemSettingDTO>().ReverseMap();
            #endregion
            #region Brand
            CreateMap<Brand, BrandRequestDTO>().ReverseMap();
            CreateMap<Brand, BrandDTO>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ReverseMap();
            CreateMap<BrandDTO, BrandRequestDTO>().ReverseMap();
            CreateMap<BrandSocialDTO, Brand>().ReverseMap();
            #endregion
            #region Feedback
            CreateMap<Feedback, FeedbackDTO>()
              .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
              .ForPath(dest => dest.User.Name, opt => opt.MapFrom(src => src.User.DisplayName))
              .ForPath(dest => dest.User.Image, opt => opt.MapFrom(src => src.User.Avatar))
              .ReverseMap();
            CreateMap<Feedback, FeedbackRequestDTO>().ReverseMap();
            #endregion
            #region campaign
            CreateMap<Campaign, CampaignDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.CampaignImages))
                .ForMember(dest => dest.Contents, opt => opt.MapFrom(src => src.CampaignContents))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                .ReverseMap();
            CreateMap<Campaign, CampaignResDto>().ReverseMap();

            CreateMap<CampaignImage, CampaignImageDto>().ReverseMap();
            CreateMap<CampaignContent, CampaignContentResDto>().ReverseMap();
            #endregion
            #region CampaignContent
            CreateMap<CampaignContentDto, CampaignContent>().ReverseMap();
            CreateMap<CampaignContent, CampaignContentResDto>().ReverseMap();
            #endregion
            #region UserDevice
            CreateMap<UserDevice, UserDeviceDTO>().ReverseMap();
            #endregion
            #region Job
            CreateMap<Job, JobRequestDTO>().ReverseMap();
            CreateMap<Job, JobDTO>().ReverseMap();
            CreateMap<Job, JobInfluencerDTO>().ReverseMap();
            #endregion
            #region JobDetails
            CreateMap<JobDetails, ChannelVideoStatDTO>().ReverseMap();
            #endregion
            #region Offer
            CreateMap<Offer, OfferRequestDTO>().ReverseMap();
            CreateMap<Offer, OfferDTO>().ReverseMap();
            #endregion
            #region Favorite
            CreateMap<Favorite, FavoriteDTO>()
            .ForMember(dest => dest.Influencer, opt => opt.MapFrom(src => src.Influencer)).ReverseMap();
            #endregion
            #region Reports
            CreateMap<ReportDTO, InfluencerReport>().ReverseMap();
            #endregion
            #region AdminAction
            CreateMap<AdminAction, AdminActionDTO>().ReverseMap();
            #endregion
            #region BannedUser
            CreateMap<BannedUser, BannedUserDTO>().ReverseMap();
            #endregion
            #region CampaignMeetingRoom
            CreateMap<CampaignMeetingRoomDTO, CampaignMeetingRoom>().ReverseMap();
            #endregion
            #region Payment
            CreateMap<PaymentHistory, PaymentHistoryDTO>().ReverseMap();

			#endregion
			#region chat
			CreateMap<CampaignChat, CampaignChatDTO>().ReverseMap();

			CreateMap<User, UserMessage>().ReverseMap();
			#endregion
		}
	}
}
