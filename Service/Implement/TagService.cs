using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Npgsql;
using Repositories;
using Serilog;
using Service.Helper;

namespace Service
{
    public class TagService : ITagService
    {
        private static readonly ITagRepository _tagRepository = new TagRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static AdminActionNotificationHelper adminActionNotificationHelper = new AdminActionNotificationHelper();
        private readonly IMapper _mapper;
        public TagService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<IEnumerable<TagDTO>> GetAllTags()
        {
            var tags = await _tagRepository.GetAlls();
            return _mapper.Map<IEnumerable<TagDTO>>(tags);
        }

        public async Task CreateTag(TagDTO tagDTO, UserDTO user)
        {
            try
            {
                tagDTO.Id = null;
                var tag = _mapper.Map<Tag>(tagDTO);
                await _tagRepository.Create(tag);

                await adminActionNotificationHelper.CreateNotification<Tag>(user, EAdminAction.Create, "Tag",tag, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    throw new InvalidOperationException("Tên của Tag phải là duy nhất và không được trùng lặp.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeleteTag(Guid id, UserDTO user)
        {
            try
            {
                var tag = await _tagRepository.GetById(id);
                if (tag == null)
                {
                    throw new KeyNotFoundException("Delete tag: Tag don't exist!");
                }
                await _tagRepository.Delete(tag);

                await adminActionNotificationHelper.CreateNotification<Tag>(user, EAdminAction.Delete, "Tag", null, tag);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    throw new InvalidOperationException("Tên của Tag phải là duy nhất và không được trùng lặp.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task UpdateTag(TagDTO tagDTO, UserDTO user)
        {
            try
            {
                if(tagDTO.Id == null)
                {
                    throw new Exception("TagID are empty when update.");
                }

                var tag = await _tagRepository.GetById(tagDTO.Id!.Value);
                if(tag == null)
                {
                    throw new KeyNotFoundException("Update tag: Tag don't exist!");
                }
                var newTag = _mapper.Map(tagDTO, tag);
                await _tagRepository.Update(tag);

                await adminActionNotificationHelper.CreateNotification<Tag>(user, EAdminAction.Update, "Tag", newTag, tag);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    throw new InvalidOperationException("Tên của Tag phải là duy nhất và không được trùng lặp.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<TagDetailDTO>> GetAllTagsWithTimeDetails()
        {
            var tags = await _tagRepository.GetAlls();
            return _mapper.Map<IEnumerable<TagDetailDTO>>(tags);
        }

        public async Task<IEnumerable<TagDetailDTO>> GetTagWithTimeDetailsById(Guid Id)
        {
            var tags = await _tagRepository.GetById(Id);
            return _mapper.Map<IEnumerable<TagDetailDTO>>(tags);
        }

    }
}
