using BusinessObjects;

namespace Service
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTags();
    }
}
