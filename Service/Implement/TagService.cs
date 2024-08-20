using AutoMapper;
using BusinessObjects.Models;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.InfluencerDTO;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
	public class TagService : ITagService
	{
		private static readonly ITagRepository _repository = new TagRepository();
		private static ILogger _loggerService = new LoggerService().GetLogger();
        private readonly IMapper _mapper;
        public TagService(IMapper mapper)
        {
			_mapper = mapper;
        }
        public async Task<IEnumerable<TagDTO>> GetAllTags()
		{
			var tags = await _repository.GetAlls();
            return _mapper.Map<List<TagDTO>>(tags); 
		}

	}
}
