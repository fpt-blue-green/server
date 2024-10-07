using AdFusionAPI.APIConfig;
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
        private readonly ITagService _tagService;
        private readonly IInfluencerService _influencerRepository;

        private readonly IMapper _mapper;
        public TagsController(ITagService tagService, IMapper mapper, IInfluencerService influencerRepository)
        {
            _mapper = mapper;
            _tagService = tagService;
            _influencerRepository = influencerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetListTag()
        {
            var result = new List<TagDTO>();
            try
            {
                var tags = await _tagService.GetAllTags();
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

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTagById(Guid id)
        {
            var result = await _tagService.GetTagById(id);
            return Ok(result);
        }

        #region Tag Management
        [AdminRequired]
        [HttpPost]
        public async Task<ActionResult> CreateTag(TagRequestDTO tagDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _tagService.CreateTag(tagDTO, user);
            return Ok();
        }

        [AdminRequired]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTag(Guid id, TagRequestDTO tagDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _tagService.UpdateTag(id, tagDTO, user);
            return Ok();
        }

        [AdminRequired]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _tagService.DeleteTag(id, user);
            return Ok();
        }
        #endregion
    }
}
