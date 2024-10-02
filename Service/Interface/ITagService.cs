using BusinessObjects;

namespace Service
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTags();
        Task<IEnumerable<TagDetailDTO>> GetAllTagsWithTimeDetails();
        Task<IEnumerable<TagDetailDTO>> GetTagWithTimeDetailsById(Guid Id);
        Task CreateTag(TagDTO tagDTO, UserDTO user);
        Task UpdateTag(TagDTO tagDTO, UserDTO user);
        Task DeleteTag(Guid id, UserDTO user);
    }
}
