using BusinessObjects;

namespace Service
{
    public interface IAdminActionService
    {
        Task<IEnumerable<AdminActionDTO>> GetAdminAction();
        Task<(byte[] fileContent, string fileName)> GetDataFile();
    }
}
