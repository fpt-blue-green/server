using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Models;

namespace Service.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Influencer
            CreateMap<Influencer, InfluencerDTO>()
                .ForMember(dest => dest.Channels, opt => opt.MapFrom(src => src.Channels))
                .ForMember(dest => dest.InfluencerTags, opt => opt.MapFrom(src => src.InfluencerTags.Select(it => it.Tag)))
                .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.Packages));
            CreateMap<Channel, ChannelDTO>();
            CreateMap<Image, ImagesDTO>();
            CreateMap<Package, PackageDTO>();
            #endregion
            #region Tag
            CreateMap<Tag, TagDTO>();
            #endregion
            #region User
            CreateMap<User, UserTokenDTO>();
            #endregion
        }
    }
}
