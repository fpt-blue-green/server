using BusinessObjects;

namespace Service
{
    public interface IAdminActionService
    {
        Task<FilterListResponse<AdminActionDTO>> GetAdminAction(FilterDTO filter);
        Task<(byte[] fileContent, string fileName)> GetDataFile();
    }
}
