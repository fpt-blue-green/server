﻿using AutoMapper;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.Enum;
using BusinessObjects.Models;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Domain;
using Service.Interface;

namespace Service.Implement
{
    public class InfluencerService : IInfluencerService
    {
        private static readonly IInfluencerRepository _repository = new InfluencerRepository();
        private static ILogger _loggerService = new LoggerService().GetLogger();
        private readonly IMapper _mapper;
<<<<<<< HEAD
=======
        private readonly ConfigManager _config;

>>>>>>> 22856a3 (update logic uploadImage & Api Influencer)
        public InfluencerService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<List<InfluencerDTO>> GetTopInfluencer()
        {
            var result = new List<InfluencerDTO>();
            try
            {
                var topInflus = (await _repository.GetAlls()).OrderBy(s => s.RateAverage).Take(10);

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
                var topInflus = (await _repository.GetAlls())
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
                var allInfluencers = await _repository.GetAlls();

                #region Filter

                if (filter.TagIds != null && filter.TagIds.Any())
                {
                    allInfluencers = allInfluencers.Where(i =>
                        i.InfluencerTags.Any(it => filter.TagIds.Contains(it.TagId))
                    ).ToList();
                }

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
                        i.FullName.Contains(filter.SearchString, StringComparison.OrdinalIgnoreCase) ||
                        i.NickName.Contains(filter.SearchString, StringComparison.OrdinalIgnoreCase)
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
                var topInflus = (await _repository.GetAlls())
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
                var topInflus = (await _repository.GetAlls())
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

        public async Task<ApiResponse<Influencer>> CreateInfluencer(Influencer influencer)
        {
            try
            {
                var entity = _mapper.Map<Influencer>(influencer);
                entity.CreatedAt = DateTime.UtcNow;
                entity.ModifiedAt = DateTime.UtcNow;

                await _repository.Create(entity);
                return new ApiResponse<Influencer>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Tạo tài khoản thành công.",
                    Data = _mapper.Map<Influencer>(entity)
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("Create New Influencer: " + ex.ToString());
                return new ApiResponse<Influencer>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = _config.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task DeleteInfluencer(Guid id)
        {
            await _repository.Delete(id);
        }

        public async Task<List<InfluencerDTO>> GetAllInfluencers()
        {
            var result = await _repository.GetAlls();
            return _mapper.Map<List<InfluencerDTO>>(result);

        }

        public async Task<InfluencerDTO> GetInfluencerById(Guid id)
        {
            var result = await _repository.GetById(id);
            return _mapper.Map<InfluencerDTO>(result);
        }


        public async Task UpdateInfluencer(Influencer influencer)
        {
            await _repository.Update(influencer);
        }
    }
}