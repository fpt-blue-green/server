using AutoMapper;
using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.InfluencerDTO;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExploreController : Controller
	{
		private readonly ITagService _tagRepository;
		private readonly IMapper _mapper;
		public ExploreController(ITagService tagRepository , IMapper mapper)
        {
            _mapper = mapper;
			_tagRepository = tagRepository;	
        }
		[HttpGet("listTag")]
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
