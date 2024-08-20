using BusinessObjects.DTOs;

namespace Service.Interface
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTags();
    }
}
