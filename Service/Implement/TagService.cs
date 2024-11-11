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

        public async Task<FilterListResponse<TagDTO>> GetAllTagsWithFilter(TagFilterDTO tagFilter)
        {
            var tags = await _tagRepository.GetAlls();

            #region Filter
            if (tagFilter.IsPremium != null)
            {
                tags = tags.Where(x => x.IsPremium == tagFilter.IsPremium);
            }
            #endregion

            int totalCount = tags.Count();
            #region paging
            int pageSize = tagFilter.PageSize;
            tags = tags
                .Skip((tagFilter.PageIndex - 1) * pageSize)
                .Take(pageSize);
            #endregion

            return new FilterListResponse<TagDTO>
            {
                Items = _mapper.Map<IEnumerable<TagDTO>>(tags),
                TotalCount = totalCount,
            };
        }

        public async Task CreateTag(TagRequestDTO tagDTO, UserDTO user)
        {
            try
            {
                var tag = _mapper.Map<Tag>(tagDTO);
                await _tagRepository.Create(tag);

                await adminActionNotificationHelper.CreateNotification<Tag>(user, EAdminActionType.Create, tag, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    throw new InvalidOperationException("Tên của thẻ phải là duy nhất và không được trùng lặp.");
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

                await adminActionNotificationHelper.CreateNotification<Tag>(user, EAdminActionType.Delete, null, tag);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    throw new InvalidOperationException("Tên của thẻ phải là duy nhất và không được trùng lặp.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task UpdateTag(Guid tagId, TagRequestDTO tagDTO, UserDTO user)
        {
            try
            {
                var tag = await _tagRepository.GetById(tagId);
                if (tag == null)
                {
                    throw new KeyNotFoundException("Update tag: Tag don't exist!");
                }
                var newTag = _mapper.Map(tagDTO, tag);
                await _tagRepository.Update(tag);

                await adminActionNotificationHelper.CreateNotification<Tag>(user, EAdminActionType.Update, newTag, tag);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    throw new InvalidOperationException("Tên của thẻ phải là duy nhất và không được trùng lặp.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<TagDTO>> GetTagById(Guid Id)
        {
            var tags = await _tagRepository.GetById(Id);
            return _mapper.Map<IEnumerable<TagDTO>>(tags);
        }

    }
}
