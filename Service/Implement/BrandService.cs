using AutoMapper;
using BusinessObjects;
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
        public async Task<string> UploadCoverAsync(IFormFile file, string folder, UserDTO user)
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
            var cover = await CloudinaryHelper.UploadImageAsync(file, folder);

            // Upload avatar to db
            //brand.CoverImage = cover.ToString();
            await _brandRepository.UpdateBrand(brand);

            _loggerService.Information("End to upload image");

            return cover.ToString();
        }
    }
}
