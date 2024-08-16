using AutoMapper;
using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.InfluencerDTO;
using BusinessObjects.ModelsDTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
			#region Influencer
			CreateMap<Influencer, InfluencerDTO>()
                .ForMember(dest => dest.Channels, opt => opt.MapFrom(src => src.Channels))
                .ForMember(dest => dest.InfluencerTags, opt => opt.MapFrom(src => src.InfluencerTags))
                .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.Packages));

            CreateMap<Channel, ChannelDTO>();
            CreateMap<Image, ImagesDTO>();
            CreateMap<InfluencerTag, InfluencerTagDTO>();
            CreateMap<Package, PackageDTO>();
			#endregion
			#region Tag
			CreateMap<Tag, TagDTO>();
			#endregion

		}
	}
}
