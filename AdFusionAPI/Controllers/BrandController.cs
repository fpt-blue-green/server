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

        [HttpPatch("cover")]
        [AuthRequired]
        public async Task<ActionResult<string>> UpdateAvatar(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var cover = await _brandService.UploadBannerAsync(file, "Cover", user);
            return Ok(cover);
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
