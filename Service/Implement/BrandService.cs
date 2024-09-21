using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Helper;
using Service.Interface;

namespace Service.Implement
{
    public class BrandService : IBrandService
    {
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static ConfigManager _configManager = new ConfigManager();
        private readonly IMapper _mapper;
        private readonly ConfigManager _config;

        public BrandService(IMapper mapper, IConfiguration config)
        {
            _mapper = mapper;
        }

        public async Task<string> CreateOrUpdateBrand(BrandRequestDTO brandRequestDTO, UserDTO user)
        {

            //Kiểm tra regex có đúng định dạng hay không
            var brandDTO = await GetBrandByUserId(user.Id);

            // Nếu chưa có, tạo mới
            if (brandDTO == null)
            {
                var newBrand = _mapper.Map<Brand>(brandRequestDTO);
                newBrand.UserId = user.Id;
                await _brandRepository.CreateBrand(newBrand);
                return "Tạo tài khoản brand thành công.";
            }
            else
            {
                // Nếu đã có, cập nhật
                _mapper.Map(brandRequestDTO, brandDTO);
                var brandUpdated = _mapper.Map<Brand>(brandDTO);
                await _brandRepository.UpdateBrand(brandUpdated);
                return "Cập nhật brand thành công.";
            }
        }

        public async Task<BrandDTO> GetBrandById(Guid id)
        {
            var result = await _brandRepository.GetBrandById(id);
            if (result == null)
            {
                throw new KeyNotFoundException();
            }
            return _mapper.Map<BrandDTO>(result);
        }

        public async Task<BrandDTO> GetBrandByUserId(Guid userId)
        {
            var result = await _brandRepository.GetByUserId(userId);
            return _mapper.Map<BrandDTO>(result);
        }

        public async Task<string> UploadCoverImgAsync(IFormFile file, string folder, UserDTO user)
        {
            _loggerService.Information("Start to upload cover image: ");

            if (file == null)
            {
                throw new Exception("Invalid File");
            }

            var brand = await _brandRepository.GetByUserId(user.Id);
            if (brand == null)
            {
                throw new InvalidOperationException("Brand không tồn tại");
            }

            // Upload ảnh
            var coverImg = await CloudinaryHelper.UploadImageAsync(file, folder, user.Id);

            // Upload avatar to db
            brand.CoverImg = coverImg.ToString();
            await _brandRepository.UpdateBrand(brand);

            _loggerService.Information("End to upload cover image");

            return brand.CoverImg;
        }
    }
}
