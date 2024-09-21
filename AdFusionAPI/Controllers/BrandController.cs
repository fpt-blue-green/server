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
        public async Task<ActionResult<BrandDTO>> GetCurrentBrand()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _brandService.GetBrandByUserId(user.Id);
            return Ok(result);
        }

        [HttpPatch("CoverImg")]
        [BrandRequired]
        public async Task<ActionResult<string>> UpdateCoverImg(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var coverImg = await _brandService.UploadCoverImgAsync(file, "Cover", user);
            return Ok(coverImg);
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
