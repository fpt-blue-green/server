using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        public BrandController(IBrandService brandService, IMapper mapper)
        {
            _brandService = brandService;
            _mapper = mapper;
        }

        [HttpGet]
        [BrandRequired]
        public async Task<ActionResult<string>> GetCurrentBrand()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.GetBrandByUserId(user.Id);
            return Ok(result);
        }

        [HttpGet("Id")]
        public async Task<ActionResult<string>> GetBrandById(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.GetBrandById(id);
            return Ok(result);
        }

        [HttpPatch("upload/banner")]
        [AuthRequired]
        public async Task<ActionResult<string>> UpdateBanner(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var banner = await _brandService.UploadBannerAsync(file, "Banner", user);
            return Ok(banner);
        }

        [HttpPut]
        [BrandRequired]
        public async Task<ActionResult<string>> CreateOrUpdateBrand([FromBody] BrandRequestDTO brandRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.CreateOrUpdateBrand(brandRequestDTO, user);
            return Ok(result);
        }
    }
}
