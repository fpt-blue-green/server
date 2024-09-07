using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Serilog;

namespace Service
{
	public class InfluencerService : IInfluencerService
	{
		private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
		//private static readonly ITagRepository _tagRepository = new TagRepository();

		private static ILogger _loggerService = new LoggerService().GetDbLogger();
		private static ISecurityService _securityService = new SecurityService();
		private static ConfigManager _configManager = new ConfigManager();
		private readonly IMapper _mapper;
		private readonly ConfigManager _config;
		public InfluencerService(IMapper mapper)
		{
			_mapper = mapper;
		}
		public async Task<List<InfluencerDTO>> GetTopInfluencer()
		{
			var result = new List<InfluencerDTO>();
			var topInflus = (await _influencerRepository.GetAlls()).OrderBy(s => s.RateAverage).Take(10);
			if (topInflus.Any())
			{
				result = _mapper.Map<List<InfluencerDTO>>(topInflus);
				return result;
			}
			return result;
		}
		public async Task<List<InfluencerDTO>> GetTopInstagramInfluencer()
		{
			var result = new List<InfluencerDTO>();
			var topInflus = (await _influencerRepository.GetAlls())
			.Where(i => i.Channels.Any(c => c.Type == (int)EPlatform.Instagram))
			.OrderBy(s => s.RateAverage)
			.Take(10);
			if (topInflus.Any())
			{
				result = _mapper.Map<List<InfluencerDTO>>(topInflus);
				return result;
			}
			return result;
		}
		public async Task<List<InfluencerDTO>> GetAllInfluencers(InfluencerFilterDTO filter)
		{
			try
			{
				var allInfluencers = await _influencerRepository.GetAlls();

				#region Filter

				/*    if (filter.TagIds != null && filter.TagIds.Any())
                    {
                        allInfluencers = allInfluencers.Where(i =>
                            i.InfluencerTags.Any(it => filter.TagIds.Contains(it.TagId))
                        ).ToList();
                    }*/
				/*    if (filter.TagIds != null && filter.TagIds.Any())
                    {
                        allInfluencers = allInfluencers.Where(i =>
                            i.InfluencerTags.Any(it => filter.TagIds.Contains(it.TagId))
                        ).ToList();
                    }*/

				if (filter.Genders != null && filter.Genders.Any())
				{
					var genderValues = filter.Genders.Select(g => (int)g).ToList();
					allInfluencers = allInfluencers.Where(i =>
						genderValues.Contains(i.Gender.GetValueOrDefault())
					).ToList();
				}
				if (filter.RateStart != null && filter.RateStart.Any())
				{
					allInfluencers = allInfluencers.Where(i => filter.RateStart.Contains((int)i.RateAverage)).OrderByDescending(i => i.AveragePrice).ToList();
				}

				if (filter.PriceFrom.HasValue && filter.PriceTo.HasValue)
				{
					allInfluencers = allInfluencers.Where(i =>
						(!filter.PriceFrom.HasValue || i.AveragePrice >= filter.PriceFrom) &&
						(!filter.PriceTo.HasValue || i.AveragePrice <= filter.PriceTo)
					).ToList();
				}
				#endregion

				#region Search
				if (!string.IsNullOrEmpty(filter.SearchString))
				{
					allInfluencers = allInfluencers.Where(i =>
						i.FullName.Contains(filter.SearchString, StringComparison.OrdinalIgnoreCase)
					// ||   i.NickName.Contains(filter.SearchString, StringComparison.OrdinalIgnoreCase)
					).ToList();
				}
				#endregion

				#region Sort

				if (filter.IsSortAcsPrice.HasValue && filter.IsSortAcsPrice.Value)
				{
					allInfluencers = allInfluencers.OrderBy(i => i.AveragePrice).ToList();
				}
				else if (filter.IsSortDesPrice.HasValue && filter.IsSortDesPrice.Value)
				{
					allInfluencers = allInfluencers.OrderByDescending(i => i.AveragePrice).ToList();
				}
				else if (filter.IsSortRate.HasValue && filter.IsSortRate.Value)
				{
					allInfluencers = allInfluencers.OrderByDescending(i => i.RateAverage).ToList();
				}
				else
				{
					allInfluencers = allInfluencers.OrderByDescending(i => i.RateAverage).OrderBy(i => i.AveragePrice).ToList();
				}
				#endregion

				#region Paging
				int pageSize = filter.PageSize;
				var pagedInfluencers = allInfluencers
					.Skip((filter.PageIndex - 1) * pageSize)
					.Take(pageSize)
					.ToList();
				#endregion

				return _mapper.Map<List<InfluencerDTO>>(pagedInfluencers);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Có lỗi xảy ra!");
			}
		}

		public async Task<List<InfluencerDTO>> GetTopTiktokInfluencer()
		{
			var result = new List<InfluencerDTO>();
			var topInflus = (await _influencerRepository.GetAlls())
			.Where(i => i.Channels.Any(c => c.Type == (int)EPlatform.Tiktok))
			.OrderBy(s => s.RateAverage)
			.Take(10);
			if (topInflus.Any())
			{
				result = _mapper.Map<List<InfluencerDTO>>(topInflus);
				return result;
			}
			return result;
		}
		public async Task<List<InfluencerDTO>> GetTopYoutubeInfluencer()
		{
			var result = new List<InfluencerDTO>();
			var topInflus = (await _influencerRepository.GetAlls())
				.Where(i => i.Channels.Any(c => c.Type == (int)EPlatform.Youtube))
				.OrderBy(s => s.RateAverage)
				.Take(10);
			if (topInflus.Any())
			{
				result = _mapper.Map<List<InfluencerDTO>>(topInflus);
				return result;
			}
			return result;
		}

        public async Task<string> CreateOrUpdateInfluencer(InfluencerRequestDTO influencerRequestDTO, UserDTO user)
        {
            // Kiểm tra xem người dùng đã có influencer hay chưa
            var influencerDTO = await GetInfluencerByUserId(user.Id);

            if (influencerDTO == null)
            {
                // Nếu chưa có, tạo mới
                var newInfluencer = _mapper.Map<Influencer>(influencerRequestDTO);
                newInfluencer.UserId = user.Id;
                await _influencerRepository.Create(newInfluencer);
                return "Tạo tài khoản influencer thành công.";
            }
            else
            {
                // Nếu đã có, cập nhật
                _mapper.Map(influencerRequestDTO, influencerDTO);
                var influencerUpdated = _mapper.Map<Influencer>(influencerDTO);
                await _influencerRepository.Update(influencerUpdated);
                return "Cập nhật influencer thành công.";
            }
        }

        public async Task DeleteInfluencer(Guid id)
		{
			await _influencerRepository.Delete(id);
		}

		public async Task<List<InfluencerDTO>> GetAllInfluencers()
		{
			var result = await _influencerRepository.GetAlls();
			return _mapper.Map<List<InfluencerDTO>>(result);
		}

		public async Task<InfluencerDTO> GetInfluencerById(Guid id)
		{
			var result = await _influencerRepository.GetById(id);
			return _mapper.Map<InfluencerDTO>(result);
		}

		public async Task<InfluencerDTO> GetInfluencerByUserId(Guid userId)
		{
			var result = await _influencerRepository.GetByUserId(userId);
			return _mapper.Map<InfluencerDTO>(result);
		}

		public async Task UpdateInfluencer(Influencer influencer)
		{
			await _influencerRepository.Update(influencer);
		}

		public async Task<List<TagDTO>> GetTagsByInfluencer(UserDTO user)
		{
			var listTagsRes = new List<TagDTO>();
			var influencer = await _influencerRepository.GetByUserId(user.Id);
			if (influencer != null)
			{
				var listTags = await _influencerRepository.GetTagsByInfluencer(influencer.Id);
				if (listTags != null && listTags.Any())
				{
					listTagsRes = _mapper.Map<List<TagDTO>>(listTags);
					return listTagsRes;
				}
			}
			return listTagsRes;
		}
		public async Task<string> UpdateTagsForInfluencer(UserDTO user, List<Guid> tagIds)
		{
			var duplicateTagIds = tagIds.GroupBy(t => t)
								.Where(g => g.Count() > 1)
								.Select(g => g.Key)
								.ToList();
			if (duplicateTagIds.Any())
			{
				throw new InvalidOperationException("Tag không được trùng lặp.");
			}
			var influencer = await _influencerRepository.GetByUserId(user.Id);
			if (influencer != null)
			{
				var influencerId = influencer.Id;
				var existingTags = await _influencerRepository.GetTagsByInfluencer(influencerId);
				var newTags = tagIds.Except(existingTags.Select(t => t.Id)).ToList();
				if (!newTags.Any())
				{
					throw new InvalidOperationException("Tất cả các tag trong danh sách đã tồn tại.");
				}
				foreach (var tagId in newTags)
				{
					await _influencerRepository.AddTagToInfluencer(influencerId, tagId);
				}
			}
			return "Cập nhật tag thành công.";
		}
	}
}