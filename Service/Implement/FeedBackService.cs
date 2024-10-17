using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
    public class FeedBackService : IFeedBackService
    {
        private static readonly IFeedBackRepository _feedbackRepository = new FeedBackRepository();
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();

        private readonly IMapper _mapper;
        public FeedBackService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<double> GetAverageRate(Guid influencerId)
        {
            var userFeedbacks = (await GetFeedBackByInfluencerId(influencerId)).ToList();
            double averageRate = 0;
            if (userFeedbacks.Any())
            {
                var validRates = userFeedbacks
                .Where(f => f.Rating.HasValue)
                .Select(f => f.Rating!.Value);
                averageRate = validRates.Average();
            }
            return Math.Round(averageRate, 2);
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedBacks()
        {
            var feedbacks = (await _feedbackRepository.GetAlls()).ToList();
            var result = _mapper.Map<IEnumerable<FeedbackDTO>>(feedbacks);
            return result;
        }

        public async Task<IEnumerable<FeedbackDTO>> GetFeedBackByInfluencerId(Guid influencerId)
        {
            var feedbacks = (await _feedbackRepository.GetAlls())
                                .Where(s => s.InfluencerId == influencerId)
                                .OrderByDescending(s => s.CreatedAt).ToList();
            var result = _mapper.Map<IEnumerable<FeedbackDTO>>(feedbacks);
            return result;
        }

        public async Task<int> GetTotalFeedbackOfInfluencer(Guid influencerId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByInfluencerId(influencerId);
            return feedbacks.Count();
        }

        public async Task CreateFeedback(Guid influencerId, FeedbackRequestDTO feedbackRequestDto, UserDTO userDTO)
        {
            // Kiểm tra rating hợp lệ
            if (feedbackRequestDto.Rating > 5 || feedbackRequestDto.Rating < 1)
            {
                throw new InvalidOperationException("Vui lòng đánh giá trong khoảng từ 1 đến 5.");
            }

            // Lấy thông tin influencer và kiểm tra feedback tồn tại
            var (influencer, existingFeedback, feedbacksForInfluencer) = await _feedbackRepository.GetInfluencerAndFeedback(userDTO.Id, influencerId);

            if (influencer.User.Id == userDTO.Id)
            {
                throw new InvalidOperationException("Bạn không thể đánh giá chính mình.");
            }

            if (existingFeedback != null)
            {
                throw new InvalidOperationException("Bạn chỉ có thể đánh giá nhà sáng tạo nội dung này một lần duy nhất.");
            }

            // Tạo feedback mới
            var feedBack = _mapper.Map<Feedback>(feedbackRequestDto);
            feedBack.InfluencerId = influencerId;
            feedBack.UserId = userDTO.Id;

            await _feedbackRepository.Create(feedBack);

            // Tính lại điểm trung bình rate cho influencer
            var totalRating = feedbacksForInfluencer.Sum(f => f.Rating) + feedbackRequestDto.Rating;
            var countRating = feedbacksForInfluencer.Count + 1;
            influencer.RateAverage = Math.Round(((decimal)totalRating! / countRating), 2);

            // Cập nhật thông tin influencer
            await _influencerRepository.Update(influencer);
        }

        public async Task UpdateFeedBack(Guid influencerId, Guid feedbackId, FeedbackRequestDTO feedbackRequest, UserDTO userDTO)
        {
            // Kiểm tra rating hợp lệ
            if (feedbackRequest.Rating > 5 || feedbackRequest.Rating < 1)
            {
                throw new InvalidOperationException("Vui lòng đánh giá trong khoảng từ 1 đến 5.");
            }

            // Lấy thông tin của Influencer dựa trên FeedbackId
            var influencer = await _influencerRepository.GetInfluencerWithFeedbackById(influencerId);

            if (influencer == null)
            {
                throw new KeyNotFoundException();
            }

            var currentFeedback = influencer.Feedbacks.FirstOrDefault(f => f.Id == feebackId);

            if (userDTO.Id != currentFeedback?.UserId)
            {
                throw new AccessViolationException();
            }

            var newFeedback = _mapper.Map(feedbackRequest, currentFeedback);

            // Update feedback
            await _feedbackRepository.Update(newFeedback);

            // Lấy mức đánh giá trung bình
            var averageRate = await GetAverageRate(influencerId);

            // Cập nhật thông tin của Influencer
            influencer.RateAverage = (decimal)averageRate;
            await _influencerRepository.Update(influencer);
        }

        public async Task DeleteFeedback(Guid influencerId, Guid feebackId, UserDTO userDTO)
        {
            // Lấy thông tin của Influencer dựa trên FeedbackId
            var influencer = await _influencerRepository.GetInfluencerWithFeedbackById(influencerId);
            var feedback = influencer?.Feedbacks?.FirstOrDefault(f => f.Id == feebackId);

            if (influencer == null || feedback == null)
            {
                throw new KeyNotFoundException();
            }
            if (userDTO.Id != feedback?.UserId && userDTO.Role != AuthEnumContainer.ERole.Admin)
            {
                throw new AccessViolationException();
            }

            // Xóa feedback
            await _feedbackRepository.Delete(feedback!);

            // Lấy mức đánh giá trung bình
            var averageRate = await GetAverageRate(influencerId);

            // Cập nhật thông tin của Influencer
            influencer.RateAverage = (decimal)averageRate;
            await _influencerRepository.Update(influencer);
        }

    }
}
