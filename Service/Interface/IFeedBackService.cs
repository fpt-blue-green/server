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
        Task CreateFeedback(FeedbackRequestDTO feedbackRequestDto, UserDTO userDTO);
        Task DeleteFeedback(Guid id, UserDTO userDTO);
    }
}
