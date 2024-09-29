using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Repositories;
using Serilog;
using Service.Helper;

namespace Service
{
	public class CampaignService : ICampaignService
	{
		private static readonly IBrandRepository _brandRepository = new BrandRepository();
		private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
		private static readonly ICampaignImageRepository _campaignImagesRepository = new CampaignImageRepository();
		private readonly IMapper _mapper;
		private static ILogger _loggerService = new LoggerService().GetDbLogger();
		public CampaignService(IMapper mapper)
		{
			_mapper = mapper;
		}
		public async Task<string> CreateCampaign(Guid userId, CampaignDTO campaignDto)
		{
			var brand = await _brandRepository.GetByUserId(userId);
			var campaign = new Campaign();
			if (campaignDto.Id == null)
			{
				//create
				campaign = _mapper.Map<Campaign>(campaignDto);
				campaign.BrandId = brand.Id;
				await _campaignRepository.Create(campaign);
			}
			else
			{
				//update
				campaign = _mapper.Map<Campaign>(campaignDto);
				campaign.BrandId = brand.Id;
				await _campaignRepository.Update(campaign);
			}
			_loggerService.Information("Tạo campaign thành công");
			return "Tạo thành công";
		}

		public async Task<CampaignDTO> GetCampaign(Guid campaignId)
		{
			var result = new CampaignDTO();
			var campaign = await _campaignRepository.GetById(campaignId);
			if (campaign == null)
			{
				throw new KeyNotFoundException("Campaign không tồn tại.");
			}

			result = _mapper.Map<CampaignDTO>(campaign);

			return result;
		}

		public async Task<List<CampaignDTO>> GetBrandCampaigns(Guid userId)
		{
			var result = new List<CampaignDTO>();
			var brand = await _brandRepository.GetByUserId(userId);
			if (brand != null)
			{
				var campaign = await _campaignRepository.GetByBrandId(brand.Id);
				if (campaign == null)
				{
					throw new KeyNotFoundException("Campaign không tồn tại.");
				}

				result = _mapper.Map<List<CampaignDTO>>(campaign);
			}
			return result;
		}

		public async Task<string> UpdateCampaign(Guid userId, CampaignDTO campaignDto)
		{
			var brand = await _brandRepository.GetByUserId(userId);
			var campaign = await _campaignRepository.GetById((Guid)campaignDto.Id);

			_mapper.Map(campaignDto, campaign);
			await _campaignRepository.Update(campaign);
			_loggerService.Information("Cập nhật campaign thành công");
			return "Cập nhật campaign thành công";
		}

		public async Task<List<CampaignBrandDto>> GetCampaignsInprogres()
		{
			var result = new List<CampaignBrandDto>();
			var campaigns = await _campaignRepository.GetAlls();
			var campaignInprogres = campaigns.Where(s => s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow).ToList();
			if (campaigns.Count() > 0)
			{
				result = _mapper.Map<List<CampaignBrandDto>>(campaigns);
			}
			return result;
		}
		/*public async Task<List<TagDTO>> GetTagsOfCampaign(Guid campaignId)
		{
			var listTagsRes = new List<TagDTO>();
			var campaign = await _campaignRepository.GetById(campaignId);
			if (campaign != null)
			{
				var listTags = await _campaignRepository.GetTagsOfCampaign(campaignId);
				if (listTags != null && listTags.Any())
				{
					listTagsRes = _mapper.Map<List<TagDTO>>(listTags);
					return listTagsRes;
				}
			}
			return listTagsRes;
		}*/
		public async Task<string> UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds)
		{
			var duplicateTagIds = tagIds.GroupBy(t => t)
								.Where(g => g.Count() > 1)
								.Select(g => g.Key)
								.ToList();
			if (duplicateTagIds.Any())
			{
				throw new InvalidOperationException("Tag không được trùng lặp.");
			}
			var campaign = await _campaignRepository.GetById(campaignId);

			if (campaign == null)
			{
				throw new InvalidOperationException("Không tìm thấy campaign.");
			}
			else
			{
				var existingTags = await _campaignRepository.GetTagsOfCampaign(campaignId);
				var deleteTags = existingTags.Where(p => !tagIds.Contains((Guid)p.Id)).ToList();
				var newTags = tagIds.Except(existingTags.Select(t => t.Id)).ToList();
				//create new tag
				if (newTags.Count > 0)
				{
					foreach (var tagId in newTags)
					{
						await _campaignRepository.AddTagToCampaign(campaignId, tagId);
					}
				}
				//delete tag
				if (deleteTags.Any())
				{
					foreach (var tag in deleteTags)
					{
						await _campaignRepository.RemoveTagOfCampaign(campaignId, tag.Id);
					}
				}

			}
			return "Cập nhật tag thành công.";
		}

		public async Task<List<string>> UploadCampaignImages(Guid campaignId, List<Guid> imageIds, List<IFormFile> contentFiles, string folder)
		{
			var contentDownloadUrls = new List<string>();


			var campaign = await _campaignRepository.GetById(campaignId);
			if (campaign == null)
			{
				throw new InvalidOperationException("Campaign không tồn tại");
			}

			// Lấy danh sách các ảnh hiện có của influencer từ DB
			var existingImages = await _campaignImagesRepository.GetByCampaignId(campaign.Id);

			// Tìm các ảnh trùng trong danh sách mới và danh sách hiện có
			var matchingImages = existingImages.Where(image => imageIds.Contains(image.Id)).ToList();

			// Tìm các ảnh không trùng trong danh sách mới và danh sách hiện có
			var unMatchingImages = existingImages.Where(image => !imageIds.Contains(image.Id)).ToList();

			// Kiểm tra nếu số ảnh không trùng và số ảnh mới nhỏ hơn 1
			if (matchingImages.Count + contentFiles.Count < 1)
			{
				throw new InvalidOperationException("Campaign phải có ít nhất 1 ảnh.");
			}

			// Kiểm tra tổng số ảnh sau khi thêm mới không được vượt quá 10
			if (matchingImages.Count + contentFiles.Count > 10)
			{
				throw new InvalidOperationException("Campaign chỉ được có tối đa 10 ảnh.");
			}

			// Nếu điều kiện hợp lệ, xóa các ảnh cũ không nằm trong danh sách mới
			foreach (var unMatchingImage in unMatchingImages)
			{
				var imagePath = CloudinaryHelper.GetValueAfterLastSlash(unMatchingImage.Url);
				var link = $"CampaignImages/{imagePath}";

				// Xóa ảnh trên cloudinary
				var deletionParams = new DeletionParams(link);
				await CloudinaryHelper.RemoveImageAsync(deletionParams);

				// Xóa ảnh từ DB
				await _campaignImagesRepository.Delete(unMatchingImage.Id);

			}

			// Thêm các ảnh mới từ contentFiles vào Cloudinary và DB
			_loggerService.Information("Start to upload content images: ");
			foreach (var file in contentFiles)
			{
				try
				{
					var contentDownloadUrl = await CloudinaryHelper.UploadImageAsync(file, folder, null);
					contentDownloadUrls.Add(contentDownloadUrl.ToString());

					var campaignImage = new CampaignImage
					{
						Id = Guid.NewGuid(),
						CampaignId = campaign.Id,
						Url = contentDownloadUrl.ToString(),
					};

					await _campaignImagesRepository.Create(campaignImage);

				}
				catch (Exception ex)
				{
					_loggerService.Error(ex, $"Error uploading content image: {file.FileName}");
					contentDownloadUrls.Add($"Error uploading content {file.FileName}: {ex.Message}");
				}
			}

			_loggerService.Information("End to upload content images: ");
			return contentDownloadUrls;
		}

	}
}
