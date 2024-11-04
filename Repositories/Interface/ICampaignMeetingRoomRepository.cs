using BusinessObjects.Models;

namespace Repositories
{
    public interface ICampaignMeetingRoomRepository
    {
        Task CreateMeetingRoom(CampaignMeetingRoom campaignMeetingRoom);
        Task DeleteMeetingRoom(CampaignMeetingRoom campaignMeetingRoom);
        Task<CampaignMeetingRoom> GetMeetingRoomByName(string name);
        Task<IEnumerable<CampaignMeetingRoom>> GetMeetingRoomsByCampaignId(Guid campaignId);
        Task UpdateMeetingRoom(CampaignMeetingRoom campaignMeetingRoom);
    }
}
