using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;

        public BrandsController(IBrandService brandService, IMapper mapper)
        {
            _brandService = brandService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDTO>> GetBrandById(Guid id)
        {
            var result = await _brandService.GetBrandById(id);
            return Ok(result);
        }

        [HttpGet("allFavorites")]
        [BrandRequired]
        public async Task<ActionResult<Favorite>> GetAllFavoriteByBrandId()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.GetAllFavoriteByBrandId(user);
            return Ok(result);
        }

        [HttpPost("{influencerId}")]
        [BrandRequired]
        public async Task<ActionResult> CreateFavorite(Guid influencerId)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _brandService.CreateFavorite(influencerId, user);
            return Ok();
        }

        [HttpDelete("{id}")]
        [BrandRequired]
        public async Task<ActionResult> DeleteFavorite(Guid id)
        {
            await _brandService.DeleteFavorite(id);
            return Ok();
        }
    }
}
