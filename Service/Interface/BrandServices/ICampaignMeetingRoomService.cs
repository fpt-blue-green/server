using BusinessObjects;

namespace Service
{
    public interface ICampaignMeetingRoomService
    {
        Task CreateRoom(RoomDataRequest dataRequest, UserDTO user);
        Task<(byte[] fileContent, string fileName)> GetLogFile(string roomName);
        Task DeleteRoomAsync(string roomName);
        Task CreateFirstTimeRoom(Guid campaignId);
        Task<FilterListResponse<CampaignMeetingRoomDTO>> GetMeetingRooms(Guid campaignId);
        Task<string> GetAccessLinkByRole(string roomName, UserDTO userDTO);
        Task UpdateRoom(RoomDataUpdateRequest updateRequest, UserDTO user);
    }
}
