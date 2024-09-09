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
            #endregion
            #region Tag
            CreateMap<Tag, TagDTO>();
            #endregion
            #region User
            CreateMap<User, UserDTO>();
            #endregion
            #region Channel
            CreateMap<ChannelStatDTO, Channel>();
            #endregion
            #region Package
            CreateMap<Package, PackageDTO>();
            CreateMap<PackageDTO, Package>();
            CreateMap<Package, PackageDtoRequest>();
            CreateMap<PackageDtoRequest, Package>();

            #endregion
        }
    }
}
