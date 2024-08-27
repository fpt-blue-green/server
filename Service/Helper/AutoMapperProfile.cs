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
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.Packages));
            CreateMap<InfluencerRequestDTO, Influencer>();
            CreateMap<Channel, ChannelDTO>();
            CreateMap<InfluencerImage, ImagesDTO>();
            CreateMap<Package, PackageDTO>();
            #endregion
            #region Tag
            CreateMap<Tag, TagDTO>();
            #endregion
            #region User
            CreateMap<User, UserDTO>();
            #endregion
        }
    }
}
