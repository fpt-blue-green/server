using BusinessObjects;

namespace Service
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTags();
        Task<IEnumerable<TagDTO>> GetTagById(Guid Id);
        Task CreateTag(TagRequestDTO tagDTO, UserDTO user);
        Task UpdateTag(Guid tagId, TagRequestDTO tagDTO, UserDTO user);
        Task DeleteTag(Guid id, UserDTO user);
    }
}
