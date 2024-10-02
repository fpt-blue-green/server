﻿using AutoMapper;
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
            CreateMap<InfluencerImage, ImageDTO>().ReverseMap();
            #endregion
            #region Tag
            CreateMap<Tag, TagDTO>().ReverseMap();
            CreateMap<Tag, TagDetailDTO>().ReverseMap();
            #endregion
            #region User
            CreateMap<User, UserDTO>().ReverseMap();
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
            CreateMap<Campaign, CampaignDTO>().ReverseMap();
            CreateMap<Campaign, CampaignBrandDto>().ReverseMap();
            #endregion
            #region CampaignContent
            CreateMap<CampaignContentDto, CampaignContent>().ReverseMap();
            CreateMap<CampaignContent, CampaignContentResDto>().ReverseMap();
            #endregion
            #region userDevice
            CreateMap<UserDevice, UserDeviceDTO>().ReverseMap();
            #endregion
        }
    }
}
