using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Enum;
using BusinessObjects.Models;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Domain;
using Service.Implement.SystemService;
using Service.Implement.SystemService;
using Service.Interface;
using Service.Interface.HelperService;
using Service.Interface.HelperService;

namespace Service.Implement
{
    public class InfluencerService : IInfluencerService
    {
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly ITagRepository _tagRepository = new TagRepository();

        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static ConfigManager _configManager = new ConfigManager();
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
            try
            {
                var topInflus = (await _influencerRepository.GetAlls()).OrderBy(s => s.RateAverage).Take(10);

                if (topInflus.Any())
                {
                    result = _mapper.Map<List<InfluencerDTO>>(topInflus);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<List<InfluencerDTO>> GetTopInstagramInfluencer()
        {
            var result = new List<InfluencerDTO>();
            try
            {
                var topInflus = (await _influencerRepository.GetAlls())
                .Where(i => i.Channels.Any(c => c.Type == (int)EPlatform.Instagram))
                .OrderBy(s => s.RateAverage)
                .Take(10);
                if (topInflus.Any())
                {
                    result = _mapper.Map<List<InfluencerDTO>>(topInflus);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                throw new Exception(ex.Message);
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
                _loggerService.Error(ex.ToString());
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<InfluencerDTO>> GetTopTiktokInfluencer()
        {
            try
            {
                var topInflus = (await _influencerRepository.GetAlls())
                .Where(i => i.Channels.Any(c => c.Type == (int)EPlatform.Tiktok))
                .OrderBy(s => s.RateAverage)
                .Take(10);
                return _mapper.Map<List<InfluencerDTO>>(topInflus);
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<InfluencerDTO>> GetTopYoutubeInfluencer()
        {
            try
            {
                var topInflus = (await _influencerRepository.GetAlls())
                .Where(i => i.Channels.Any(c => c.Type == (int)EPlatform.Youtube))
                .OrderBy(s => s.RateAverage)
                .Take(10);
                return _mapper.Map<List<InfluencerDTO>>(topInflus);
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse<Influencer>> CreateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token)
        public async Task<ApiResponse<Influencer>> CreateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token)
        {
            try
            {
                var tokenDescrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDescrypt == null)
                {
                    return new ApiResponse<Influencer>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = _configManager.TokenInvalidErrorMessage,
                        Data = null
                    };
                }

                var user = JsonConvert.DeserializeObject<UserDTO>(tokenDescrypt);

                var newInfluencer = _mapper.Map<Influencer>(influencerRequestDTO);
                newInfluencer.Id = Guid.NewGuid();
                newInfluencer.UserId = user.Id;
                newInfluencer.IsPublish = true;
                newInfluencer.CreatedAt = DateTime.UtcNow;

                await _repository.Create(newInfluencer);

                return new ApiResponse<Influencer>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Tạo tài khoản thành công.",
                    Data = _mapper.Map<Influencer>(newInfluencer)
                    Data = _mapper.Map<Influencer>(newInfluencer)
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("Create New Influencer: " + ex.ToString());
                return new ApiResponse<Influencer>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _config.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<Influencer>> UpdateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token)
        {
            try
            {
                var tokenDescrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDescrypt == null)
                {
                    return new ApiResponse<Influencer>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = _configManager.TokenInvalidErrorMessage,
                        Data = null
                    };
                }

                var user = JsonConvert.DeserializeObject<UserDTO>(tokenDescrypt);

                var influencer = GetInfluencerByUserId(user.Id);
                if (influencer == null)
                {
                    return new ApiResponse<Influencer>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = _configManager.InvalidInfluencer,
                        Data = null
                    };
                }

                var influencerUpdated = _mapper.Map<Influencer>(influencerRequestDTO);
                await _repository.Update(influencerUpdated);

                return new ApiResponse<Influencer>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Update influencer thành công.",
                    Data = _mapper.Map<Influencer>(influencer)
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("Update Influencer: " + ex.ToString());
                return new ApiResponse<Influencer>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _config.SeverErrorMessage,
                    Data = null
                };
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
            var result = await _repository.GetByUserId(userId);
            return _mapper.Map<InfluencerDTO>(result);
        }

        public async Task UpdateInfluencer(Influencer influencer)
        {
            var result = await _influencerRepository.GetByUserId(userId);
            return _mapper.Map<InfluencerDTO>(result);
        }

        public async Task<ApiResponse<List<TagDTO>>> GetTagsByInfluencer(string token)
        {
            var listTagsRes = new List<TagDTO>();
            try
            {
                var user = await getUserByTokenAsync(token);
                if (user != null)
                {
                    var influencer = await _influencerRepository.GetByUserId(user.Id);
                    if (influencer != null)
                    {
                        var listTags = await _influencerRepository.GetTagsByInfluencer(influencer.Id);
                        if (listTags != null && listTags.Any())
                        {
                            listTagsRes = _mapper.Map<List<TagDTO>>(listTags);
                        }
                    }
                }
            }
            catch
            {
                return new ApiResponse<List<TagDTO>>()
                {
                    Data = null,
                    Message = "Lấy danh sách thất bại",
                    StatusCode = EHttpStatusCode.BadRequest,
                };
            }
            return new ApiResponse<List<TagDTO>>()
            {
                Data = listTagsRes,
                Message = "Lấy danh sách thành công",
                StatusCode = EHttpStatusCode.OK,
            };
        }

       /* public async Task<ApiResponse<object>> AddTagToInfluencer(string token, List<Guid> tagIds)
        {
            try
            {
				var duplicateTagIds = tagIds.GroupBy(t => t)
									.Where(g => g.Count() > 1)
									.Select(g => g.Key)
									.ToList();
				if (duplicateTagIds.Any())
				{
					return new ApiResponse<object>
					{
						Data = null,
						Message = "Tag không được trùng lặp.",
						StatusCode = EHttpStatusCode.BadRequest,
					};
				}

				var user = await getUserByTokenAsync(token);
                if (user != null)
                {
                    var influencer = await _influencerRepository.GetByUserId(user.Id);
                    if (influencer != null)
                    {
						var influencerId = influencer.Id;
						var existingTags = await _influencerRepository.GetTagsByInfluencer(influencerId);
						var newTags = tagIds.Except(existingTags.Select(t => t.Id)).ToList();
						if (!newTags.Any())
						{
							return new ApiResponse<object>
							{
								Data = null,
								Message = "Tất cả các tag trong danh sách đã tồn tại.",
								StatusCode = EHttpStatusCode.BadRequest,
							};
						}
						foreach (var tagId in newTags)
                        {
                            await _influencerRepository.AddTagToInfluencer(influencerId, tagId);
                        }
                        return new ApiResponse<object>
                        {
                            Data = null,
                            Message = "Tạo tag thành công",
                            StatusCode = EHttpStatusCode.OK,
                        };
                    }
                }
            }catch{
            }
            return new ApiResponse<object>
            {
                Data = null,
                Message = "Tạo tag thành công",
                StatusCode = EHttpStatusCode.OK,
            };
        }*/
        public async Task<User> getUserByTokenAsync(string token)
        {
            try
            {
                var tokenDecrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDecrypt == null)
                {
                    throw new Exception("Invalid token.");
                }
                var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
                return user;
            }catch(Exception ex)
            {
                throw new Exception();
            }
           
        }

        public async Task<ApiResponse<object>> UpdateTagsForInfluencer(string token, List<Guid> tagIds)
        {
            try
            {
				var duplicateTagIds = tagIds.GroupBy(t => t)
									.Where(g => g.Count() > 1)
									.Select(g => g.Key)
									.ToList();
				if (duplicateTagIds.Any())
				{
					return new ApiResponse<object>
					{
						Data = null,
						Message = "Tag không được trùng lặp.",
						StatusCode = EHttpStatusCode.BadRequest,
					};
				}
				var user = await getUserByTokenAsync(token);
                if (user != null)
                {
                    var influencer = await _influencerRepository.GetByUserId(user.Id);
                    if (influencer != null)
                    {
                        await _influencerRepository.UpdateTagsForInfluencer(influencer.Id, tagIds);
                        return new ApiResponse<object>
                        {
                            Data = null,
                            Message = "Cập nhật thành công",
                            StatusCode = EHttpStatusCode.OK,
                        };
                    }
                }
            }catch 
            {
            }
            return new ApiResponse<object>
            {
                Data = null,
                Message = "Cập nhật thất bại ",
                StatusCode = EHttpStatusCode.BadRequest,
            };
        }
    }
}