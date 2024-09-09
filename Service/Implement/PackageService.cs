using BusinessObjects;
using Serilog;
using Repositories;
using AutoMapper;
using BusinessObjects.Models;
using System.Linq;

namespace Service
{
	public class PackageService : IPackageService
	{
		private static readonly IPackageRepository _packageRepository = new PackageRepository();
		private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();

		private readonly IMapper _mapper;
		private static ILogger _loggerService = new LoggerService().GetDbLogger();
		public PackageService(IMapper mapper)
		{
			_mapper = mapper;
		}
		public async Task<string> CreatePackage(Guid userId, List<PackageDTO> packages)
		{
			var createPackages = new List<PackageDTO>();
			var updatePackages = new List<PackageDTO>();
			var deletePackageList = new List<PackageDTO>();
			//get all package of influ
			var allPackages = await GetInfluPackages(userId);
			var allPackageIds = allPackages.Select(p => p.Id).ToList();

		
			foreach (var package in packages)
			{
				if (package.Id.HasValue)
				{
					//nếu thằng mô có id thì allway update
					updatePackages.Add(package);
				}
				else
				{
					//thằng mô ko có id thì create
					createPackages.Add(package);
				}
			}
			var influencerId = (await _influencerRepository.GetByUserId(userId)).Id;
			if (influencerId == null)
			{
				_loggerService.Information("Influencer không tồn tại.");
				throw new InvalidOperationException("Influencer không tồn tại.");
			}
			if (updatePackages.Any())
			{
				foreach (var packageDTO in updatePackages)
				{
					// Truy xuất package hiện có từ repository
					var existingPackage = await _packageRepository.GetById(packageDTO.Id.Value);
					if (existingPackage != null)
					{
						// Sử dụng AutoMapper để cập nhật các thuộc tính từ DTO vào thực thể hiện có
						_mapper.Map(packageDTO, existingPackage);
						existingPackage.InfluencerId = influencerId; // Đảm bảo InfluencerId được gán

						// Gọi phương thức cập nhật trên repository
						await _packageRepository.Update(existingPackage);
					}
					else
					{
						_loggerService.Information($"Package với ID {packageDTO.Id.Value} không tồn tại.");
						throw new InvalidOperationException($"Package với ID {packageDTO.Id.Value} không tồn tại.");
					}
				}
			}
			if (createPackages.Any())
			{
				var packageNeedCreate = _mapper.Map<List<Package>>(createPackages);
				
				if (packageNeedCreate == null || packageNeedCreate.Count == 0)
				{
					_loggerService.Information("Tạo package thất bại.");
					throw new InvalidOperationException("Vui lòng tạo ít nhất 1 package.");
				}
				packageNeedCreate.ForEach(package => package.InfluencerId = influencerId);

				await _packageRepository.CreateList(packageNeedCreate);
				_loggerService.Information("Tạo package thành công");
			}
			// Tìm các package có trong allPackages nhưng không có trong listPackages
			var listDtoPackageIds = packages //1,2
				.Where(p => p.Id.HasValue)
				.Select(p => p.Id.Value)
				.ToList();
			if (listDtoPackageIds.Count > 0)
			{
				deletePackageList = allPackages.Where(p => !listDtoPackageIds.Contains((Guid)p.Id)).ToList();
				if (deletePackageList.Any())
				{
					var packageNeedDelete = _mapper.Map<List<Package>>(deletePackageList);

					foreach (var package in packageNeedDelete)
					{
						await _packageRepository.Delete(package.Id);
					}
					_loggerService.Information("Xoá package không còn tồn tại thành công");
				}
			}
			return "Tạo thành công";
		}

		public async Task<PackageDTO> GetInfluPackage(Guid packageId, Guid userId)
		{
			var package = await _packageRepository.GetById(packageId);
			if (package == null)
			{
				_loggerService.Information($"get package {packageId} thất bại");
				throw new InvalidOperationException("Package không tồn tại.");

			}
			_loggerService.Information($"get package {packageId} thành công");
			return _mapper.Map<PackageDTO>(package);
		}

		public async Task<List<PackageDTO>> GetInfluPackages(Guid userId)
		{
			var influencerId = (await _influencerRepository.GetByUserId(userId)).Id;
			if (influencerId == null)
			{
				_loggerService.Information("Influencer không tồn tại.");
				throw new InvalidOperationException("Influencer không tồn tại.");
			}
			var packages = (await _packageRepository.GetAlls()).Where(s => s.InfluencerId == influencerId);
			if (packages == null || !packages.Any())
			{
				_loggerService.Information($"get packages của influencer: {userId} không có value : count = 0");
				return new List<PackageDTO>();
			}
			_loggerService.Information($"get packages của influencer: {userId} thành công");
			return _mapper.Map<List<PackageDTO>>(packages);
		}

		public async Task<string> UpdateInfluencerPackage(Guid userId, Guid packageId, PackageDtoRequest packageDTO)
		{
			var package = await _packageRepository.GetById(packageId);
			var influencerId = (await _influencerRepository.GetByUserId(userId)).Id;
			if (influencerId == null)
			{
				_loggerService.Information("Influencer không tồn tại.");
				throw new InvalidOperationException("Influencer không tồn tại.");
			}
			if (package == null)
			{
				throw new InvalidOperationException("Không tìm thấy package.");
			}
			if (package.InfluencerId == influencerId)
			{
				_mapper.Map(packageDTO, package);
				await _packageRepository.Update(package);
				_loggerService.Information($"cập nhật package của influencer: {userId} thành công");
			}
			else
			{
				_loggerService.Information($"cập nhật package của influencer: {userId} thất bại.");
				throw new InvalidOperationException("Không thể cập nhật.");
			}
			return "Cập nhật thành công";


		}
	}
}
