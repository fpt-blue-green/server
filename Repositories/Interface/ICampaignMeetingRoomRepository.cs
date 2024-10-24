using BusinessObjects.Models;

namespace Repositories
{
    public interface ICampaignMeetingRoomRepository
    {
        Task CreateMeetingRoom(CampaignMeetingRoom campaignMeetingRoom);
        Task DeleteMeetingRoom(CampaignMeetingRoom campaignMeetingRoom);
        Task<CampaignMeetingRoom> GetMeetingRoomByName(string name);
        Task<CampaignMeetingRoom> GetMeetingRoomByCampaignId(Guid campaignId);
    }
}
