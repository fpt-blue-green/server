using BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.Models;

namespace Service
{
    public interface IFeedBackService
    {
        Task<IEnumerable<FeedbackDTO>> GetAllFeedBacks();
        Task<IEnumerable<FeedbackDTO>> GetFeedBackByInfluencerId(Guid influencerId);
        Task<double> GetAverageRate(Guid userId);
        Task CreateFeedback(Guid influencerId, FeedbackRequestDTO feedbackRequestDto, UserDTO userDTO);
        Task DeleteFeedback(Guid influencerId, Guid feebackId, UserDTO userDTO);
    }
}
