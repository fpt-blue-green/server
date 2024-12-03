using BusinessObjects;

namespace Service
{
    public interface ITagService
    {
        Task<FilterListResponse<TagDTO>> GetAllTagsWithFilter(TagFilterDTO tagFilter);
        Task<IEnumerable<TagDTO>> GetAllTags();
        Task<TagDTO> GetTagById(Guid Id);
        Task CreateTag(TagRequestDTO tagDTO, UserDTO user);
        Task UpdateTag(Guid tagId, TagRequestDTO tagDTO, UserDTO user);
        Task DeleteTag(Guid id, UserDTO user);
    }
}
