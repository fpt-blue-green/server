using AutoMapper;
using BusinessObjects.DTOs;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Interface;

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
