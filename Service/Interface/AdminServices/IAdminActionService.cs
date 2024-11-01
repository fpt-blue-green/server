using BusinessObjects;

namespace Service
{
    public interface IAdminActionService
    {
        Task<IEnumerable<AdminActionDTO>> GetAdminAction(FilterDTO filter);
        Task<(byte[] fileContent, string fileName)> GetDataFile();
    }
}
