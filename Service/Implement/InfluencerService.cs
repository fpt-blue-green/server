using AutoMapper;
using BusinessObjects;
using BusinessObjects.Helper;
using BusinessObjects.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Repositories;
using Serilog;
using Service.Helper;
using Supabase;
using System.Reflection;
using System.Text.RegularExpressions;
using static BusinessObjects.JobEnumContainer;
using static Supabase.Gotrue.Constants;

namespace Service
{
    public class InfluencerService : IInfluencerService
    {
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly IInfluencerImageRepository _influencerImagesRepository = new InfluencerImageRepository();
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        //private static readonly ITagRepository _tagRepository = new TagRepository();

        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static ConfigManager _configManager = new ConfigManager();
        private readonly IMapper _mapper;
        private readonly Client _supabase;

        public InfluencerService(IMapper mapper, Client supabase)
        {
            _mapper = mapper;
            _supabase = supabase;
        }
        public async Task<List<InfluencerDTO>> GetTopInfluencer()
        {
            var result = new List<InfluencerDTO>();
            var topInflus = (await _influencerRepository.GetAlls()).OrderBy(s => s.RateAverage).Where(i => i.IsPublish == true).Take(10);
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
            .Where(i => i.Channels.Any(c => c.Platform == (int)EPlatform.Instagram) && i.IsPublish == true)
            .OrderBy(s => s.RateAverage)
            .Take(10);
            if (topInflus.Any())
            {
                result = _mapper.Map<List<InfluencerDTO>>(topInflus);
                return result;
            }
            return result;
        }
        public async Task<FilterListResponse<InfluencerDTO>> GetAllInfluencers(InfluencerFilterDTO filter)
        {
            try
            {
                var allInfluencers = (await _influencerRepository.GetAlls()).Where(i => i.IsPublish == true);

                #region Filter

                if (filter.TagIds != null && filter.TagIds.Any())
                {
                    allInfluencers = allInfluencers.Where(i =>
                        i.Tags.Any(it => filter.TagIds.Contains(it.Id))
                    );
                }
                if (filter.Genders != null && filter.Genders.Any())
                {
                    var genderValues = filter.Genders.Select(g => (int)g);
                    allInfluencers = allInfluencers.Where(i =>
                        genderValues.Contains(i.Gender)
                    );
                }
                if (filter.Platforms != null && filter.Platforms.Any())
                {
                    allInfluencers = allInfluencers.Where(i =>
                    i.Channels.Any(c => filter.Platforms.Contains((EPlatform)c.Platform)));
                }
                if (filter.RateStart.HasValue)
                {
                    allInfluencers = allInfluencers.Where(i => i.RateAverage >= filter.RateStart).OrderByDescending(i => i.AveragePrice);
                }

                if (filter.PriceFrom.HasValue && filter.PriceTo.HasValue)
                {
                    allInfluencers = allInfluencers.Where(i =>
                        (!filter.PriceFrom.HasValue || i.AveragePrice >= filter.PriceFrom) &&
                        (!filter.PriceTo.HasValue || i.AveragePrice <= filter.PriceTo)
                    );
                }
                #endregion

                #region Search
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    allInfluencers = allInfluencers.Where(i =>
                        TextComparator.ContainsIgnoreCaseAndDiacritics(i.FullName, filter.Search)
                    );
                }
                #endregion

                #region Sort
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    var propertyInfo = typeof(Influencer).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        allInfluencers = filter.IsAscending.HasValue && filter.IsAscending.Value
                            ? allInfluencers.OrderBy(i => propertyInfo.GetValue(i, null))
                            : allInfluencers.OrderByDescending(i => propertyInfo.GetValue(i, null));
                    }
                }
                #endregion

                #region Paging
                int pageSize = filter.PageSize;
                var pagedInfluencers = allInfluencers
                    .Skip((filter.PageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                #endregion

                return new FilterListResponse<InfluencerDTO>
                {
                    TotalCount = allInfluencers.Count(),
                    Items = _mapper.Map<List<InfluencerDTO>>(pagedInfluencers)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Có lỗi xảy ra!", ex);
            }
        }

        public async Task<List<InfluencerDTO>> GetTopTiktokInfluencer()
        {
            var result = new List<InfluencerDTO>();
            var topInflus = (await _influencerRepository.GetAlls())
            .Where(i => i.Channels.Any(c => c.Platform == (int)EPlatform.Tiktok) && i.IsPublish == true)
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
                .Where(i => i.Channels.Any(c => c.Platform == (int)EPlatform.Youtube) && i.IsPublish == true)
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
            //Kiểm tra xem slug đã được sử dụng hay chưa
            if (!Regex.IsMatch(influencerRequestDTO.Slug, _configManager.SlugRegex))
            {
                throw new InvalidOperationException("Tên người dùng không hợp lệ.");
            }

            //Kiểm tra regex có đúng định dạng hay không
            var influencerDTO = await GetInfluencerByUserId(user.Id);

            // Nếu chưa có, tạo mới
            if (influencerDTO == null)
            {
                //Kiểm trả xem slug đã được sử dụng hay chưa
                var result = await _influencerRepository.GetBySlug(influencerRequestDTO.Slug);
                if (result != null)
                {
                    throw new InvalidOperationException("Tên người dùng đã được sử dụng. Vui lòng chọn tên người dùng khác.");
                }
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

        public async Task<InfluencerDTO> GetInfluencerBySlug(string slug)
        {
            var result = await _influencerRepository.GetBySlug(slug);
            if (result == null || result.IsPublish == false)
            {
                throw new KeyNotFoundException();
            }
            return _mapper.Map<InfluencerDTO>(result);
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
                throw new InvalidOperationException("Thẻ không được trùng lặp.");
            }
            var influencer = await _influencerRepository.GetByUserId(user.Id);
            if (influencer == null)
            {
                throw new InvalidOperationException(_configManager.ProfileNotComplete);
            }
            else
            {
                var influencerId = influencer.Id;
                var existingTags = await _influencerRepository.GetTagsByInfluencer(influencerId);
                var deleteTags = existingTags.Where(p => !tagIds.Contains((Guid)p.Id)).ToList();
                var newTags = tagIds.Except(existingTags.Select(t => t.Id)).ToList();
                //create new tag
                if (newTags.Count > 0)
                {
                    foreach (var tagId in newTags)
                    {
                        await _influencerRepository.AddTagToInfluencer(influencerId, tagId);
                    }
                }
                //delete tag
                if (deleteTags.Any())
                {
                    foreach (var tag in deleteTags)
                    {
                        await _influencerRepository.RemoveTagOfInfluencer(influencerId, tag.Id);
                    }
                }

            }
            return "Cập nhật tag thành công.";
        }

        public async Task<List<string>> UploadContentImages(List<Guid> imageIds, List<IFormFile> contentFiles, UserDTO user, string folder)
        {
            var contentDownloadUrls = new List<string>();

            var influencer = await _influencerRepository.GetByUserId(user.Id);
            if (influencer == null)
            {
                throw new InvalidOperationException("Nhà sáng tạo nội dung không tồn tại");
            }

            // Lấy danh sách các ảnh hiện có của influencer từ DB
            var existingImages = await _influencerImagesRepository.GetByInfluencerId(influencer.Id);

            // Tìm các ảnh trùng trong danh sách mới và danh sách hiện có
            var matchingImages = existingImages.Where(image => imageIds.Contains(image.Id)).ToList();

            // Tìm các ảnh không trùng trong danh sách mới và danh sách hiện có
            var unMatchingImages = existingImages.Where(image => !imageIds.Contains(image.Id)).ToList();

            // Kiểm tra nếu số ảnh không trùng và số ảnh mới nhỏ hơn 3
            if (matchingImages.Count + contentFiles.Count < 3)
            {
                throw new InvalidOperationException("Nhà sáng tạo nội dung phải có ít nhất 3 ảnh.");
            }

            // Kiểm tra tổng số ảnh sau khi thêm mới không được vượt quá 10
            if (matchingImages.Count + contentFiles.Count > 10)
            {
                throw new InvalidOperationException("Nhà sáng tạo nội dung chỉ được có tối đa 10 ảnh.");
            }

            // Nếu điều kiện hợp lệ, xóa các ảnh cũ không nằm trong danh sách mới
            foreach (var unMatchingImage in unMatchingImages)
            {
                var imagePath = CloudinaryHelper.GetValueAfterLastSlash(unMatchingImage.Url);
                var link = $"InfluencerImages/{imagePath}";

                // Xóa ảnh trên cloudinary
                var deletionParams = new DeletionParams(link);
                await CloudinaryHelper.RemoveImageAsync(deletionParams);

                // Xóa ảnh từ DB
                await _influencerImagesRepository.Delete(unMatchingImage.Id);

            }

            // Thêm các ảnh mới từ contentFiles vào Cloudinary và DB
            _loggerService.Information("Start to upload content images: ");
            foreach (var file in contentFiles)
            {
                try
                {
                    var contentDownloadUrl = await CloudinaryHelper.UploadImageAsync(file, folder, null);
                    contentDownloadUrls.Add(contentDownloadUrl.ToString());

                    var influencerImage = new InfluencerImage
                    {
                        Id = Guid.NewGuid(),
                        InfluencerId = influencer.Id,
                        Url = contentDownloadUrl.ToString(),
                    };

                    await _influencerImagesRepository.Create(influencerImage);

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

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            // Định dạng regex cho số điện thoại (10 hoặc 11 số, bắt đầu bằng số 0)
            string pattern = @"^(\+84|0[3|5|7|8|9])[0-9]{8}$";

            return Regex.IsMatch(phoneNumber, pattern);
        }

        public async Task<bool> SendPhoneOtp(string phone)
        {
            string phoneNumber = phone;
            if (phone.StartsWith("0"))
            {
                phoneNumber = "+84" + phone.Substring(1);
            }
            await _supabase.Auth.SignIn(SignInType.Phone, phoneNumber);
            return true;
        }

        public async Task<bool> VerifyPhoneOtp(UserDTO user, string phone, string otp)
        {
            try
            {
                string phoneNumber = phone;
                // Phone: +84775428404, OTP: 123456
                if (phone.StartsWith("0"))
                {
                    phoneNumber = "+84" + phone.Substring(1);
                }
                await _supabase.Auth.VerifyOTP(phoneNumber, otp, MobileOtpType.SMS);

                var influencer = await _influencerRepository.GetByUserId(user.Id);
                influencer.Phone = phone;
                influencer.IsPublish = true;
                var influencerUpdated = _mapper.Map<Influencer>(influencer);
                await _influencerRepository.Update(influencerUpdated);
            }
            catch
            {
                throw new InvalidOperationException("Mã xác thực không hợp lệ hoặc đã hết hạn.");
            }
            return true;
        }

        public async Task<FilterListResponse<InfluencerJobDTO>> GetInfluencerWithJobByCampaginId(Guid campaignId, InfluencerJobFilterDTO filter, UserDTO user)
        {
            var influencers = await _influencerRepository.GetInfluencerJobByCampaignId(campaignId);

            var influencerJobDTOs = influencers.Select(influencer =>
            {
                // Ánh xạ influencer sang InfluencerJobDTO
                var influencerJobDTO = _mapper.Map<InfluencerJobDTO>(influencer);

                // Lấy danh sách JobDTO cho từng influencer và áp dụng filter
                influencerJobDTO.Jobs = influencer.Jobs
                    .Where(job => (filter.JobStatuses == null || !filter.JobStatuses.Any() || filter.JobStatuses.Contains((EJobStatus)job.Status)) &&
                                  (filter.OfferStatuses == null || !filter.OfferStatuses.Any() || job.Offers.Any(offer => filter.OfferStatuses.Contains((EOfferStatus)offer.Status))))
                    .Select(job =>
                    {
                        // Lấy offer mới nhất cho từng job
                        var latestOffer = job.Offers
                            .OrderByDescending(offer => offer.CreatedAt)
                            .FirstOrDefault();

                        // Ánh xạ Job sang JobInfluencerDTO
                        var jobDTO = _mapper.Map<JobInfluencerDTO>(job);

                        // Chỉ ánh xạ offer mới nhất vào JobDTO
                        if (jobDTO != null && latestOffer != null)
                        {
                            jobDTO.Offer = _mapper.Map<OfferDTO>(latestOffer);
                        }

                        return jobDTO;
                    }).ToList()!;

                return influencerJobDTO;
            }).ToList();

            // Lọc Influencer nếu không còn Jobs
            influencerJobDTOs = influencerJobDTOs.Where(i => i.Jobs.Any()).ToList();

            int totalCount = influencerJobDTOs.Count();

            #region paging
            int pageSize = filter.PageSize;
            influencerJobDTOs = influencerJobDTOs
                .Skip((filter.PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            #endregion

            return new FilterListResponse<InfluencerJobDTO>
            {
                TotalCount = totalCount,
                Items = influencerJobDTOs
            };
        }


    }
}