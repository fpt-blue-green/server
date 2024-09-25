using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
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
    }
}
