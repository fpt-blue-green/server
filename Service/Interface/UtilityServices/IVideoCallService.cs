namespace Service
{
    public interface IVideoCallService
    {
        Task<String> CreateRoom(string roomName);
        Task<(byte[] fileContent, string fileName)> GetLogFile();
        Task DeleteRoomAsync(string roomName);
    }
}
