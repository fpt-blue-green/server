using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : Controller
    {
        private readonly ITagService _tagRepository;
        private readonly IInfluencerService _influencerRepository;

        private readonly IMapper _mapper;
        public TagsController(ITagService tagRepository, IMapper mapper, IInfluencerService influencerRepository)
        {
            _mapper = mapper;
            _tagRepository = tagRepository;
            _influencerRepository = influencerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetListTag()
        {
            var result = new List<TagDTO>();
            try
            {
                var tags = await _tagRepository.GetAllTags();
                if (tags.Any())
                {
                    foreach (var item in tags)
                    {
                        result = _mapper.Map<List<TagDTO>>(tags);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }

    }
}
