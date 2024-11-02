

using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
	public class GroupChatService : IGroupChatService
	{
		private readonly IGroupUserChatRepository _groupUserChatRepository = new GroupUserChatRepository();
		private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<AutoMapperProfile>();
		}).CreateMapper();
		public async Task CreateOrSaveMessageAsync(CampaignChatDTO userChat)
		{
			var campaignChat = _mapper.Map<CampaignChat>(userChat);
			await _groupUserChatRepository.CreateOrSaveMessageAsync(campaignChat);
		}

		public async Task<List<CampaignChatDTO>> GetGroupMessageAsync(string roomName)
		{
			var campaignChats = await _groupUserChatRepository.GetGroupMessageAsync(roomName);
			var campaignChatDtos = _mapper.Map<List<CampaignChatDTO>>(campaignChats);
			return campaignChatDtos;
		}

		public async Task<CampaignChatDTO> GetLastMessage(Guid campaignId, Guid senderId)
		{
			var chat = await _groupUserChatRepository.GetLastMessage(campaignId, senderId);
			return _mapper.Map<CampaignChatDTO>(chat);
		}
	}
}
