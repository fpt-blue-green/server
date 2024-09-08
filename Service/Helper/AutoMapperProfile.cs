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
            CreateMap<Influencer, InfluencerDTO>().ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar)).ReverseMap();
            CreateMap<InfluencerRequestDTO, InfluencerDTO>();
            CreateMap<InfluencerRequestDTO, Influencer>();
            CreateMap<Channel, ChannelDTO>();
            CreateMap<InfluencerImage, ImageDTO>();
            CreateMap<Package, PackageDTO>();
            #endregion
            #region Tag
            CreateMap<Tag, TagDTO>();
            #endregion
            #region User
            CreateMap<User, UserDTO>();
            #endregion
            #region Channel
            CreateMap<ChannelStatDTO, Channel>();
            CreateMap<Channel, ChannelDTO>();
            #endregion
            #region SystemSetting
            CreateMap<SystemSetting, SystemSettingDTO>();
            #endregion
        }
    }
}
