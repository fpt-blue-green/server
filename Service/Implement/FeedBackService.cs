using AutoMapper;
using BusinessObjects;
using BusinessObjects.DTOs;
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
            var feedbacks = (await _feedbackRepository.GetAlls()).Where(s => s.InfluencerId == influencerId).ToList();
            var result = _mapper.Map<IEnumerable<FeedbackDTO>>(feedbacks);
            return result;
        }

        public async Task CreateFeedback(FeedbackRequestDTO feedbackRequestDto, UserDTO userDTO)
        {
            if (feedbackRequestDto.Rating > 5 || feedbackRequestDto.Rating < 1)
            {
                throw new InvalidOperationException("Vui lòng rating trong khoảng từ 1 đến 5.");
            }

            var feedBack = _mapper.Map<Feedback>(feedbackRequestDto);
            feedBack.UserId = userDTO.Id;

            // Tạo feedback
            await _feedbackRepository.Create(feedBack);

            // Lấy average rate
            var averageRate = await GetAverageRate(feedbackRequestDto.InfluencerId);

            // Lấy influencer và cập nhật rate
            var influencer = await _influencerRepository.GetById(feedbackRequestDto.InfluencerId);
            influencer.RateAverage = (decimal)averageRate;

            // Cập nhật thông tin influencer
            await _influencerRepository.Update(influencer);
        }

        public async Task DeleteFeedback(Guid id, UserDTO userDTO)
        {
            // Lấy thông tin của Influencer dựa trên FeedbackId
            var influencer = await _influencerRepository.GetInfluencerByFeedbackID(id);

            if (influencer == null)
            {
                throw new KeyNotFoundException();
            }
            if(userDTO.Id != influencer.Feedbacks.FirstOrDefault()?.UserId && userDTO.Role != AuthEnumContainer.ERole.Admin)
            {
                throw new AccessViolationException();
            }

            // Xóa feedback
            await _feedbackRepository.Delete(id);

            // Lấy mức đánh giá trung bình
            var averageRate = await GetAverageRate(influencer.Id);

            // Cập nhật thông tin của Influencer
            influencer.RateAverage = (decimal)averageRate;
            await _influencerRepository.Update(influencer);
        }

    }
}
