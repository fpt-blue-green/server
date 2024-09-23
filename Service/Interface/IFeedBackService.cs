using BusinessObjects;

namespace Service
{
    public interface IFeedBackService
    {
        Task<IEnumerable<FeedbackDTO>> GetAllFeedBacks();
        Task<IEnumerable<FeedbackDTO>> GetFeedBackByInfluencerId(Guid influencerId);
        Task<double> GetAverageRate(Guid userId);
        Task CreateFeedback(Guid influencerId, FeedbackRequestDTO feedbackRequestDto, UserDTO userDTO);
        Task DeleteFeedback(Guid influencerId, Guid feebackId, UserDTO userDTO);
        Task UpdateFeedBack(Guid influencerId, Guid feebackId, FeedbackRequestDTO feedbackRequest, UserDTO userDTO);
    }
}
