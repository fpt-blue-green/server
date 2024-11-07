
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
    public class CampaignChatService : ICampaignChatService
	{
		private readonly ICampaignChatRepository _groupUserChatRepository = new CampaignChatRepository();
        private readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<AutoMapperProfile>();
		}).CreateMapper();
		public async Task CreateCampaignChatRoom(CampaignChatResDTO campaignChat)
        {
            var campaign = await _campaignRepository.GetById(campaignChat.CampaignId);
			var room = new CampaignChat
			{
				CampaignId = campaignChat.CampaignId,
				CreatedAt = DateTime.UtcNow,
				RoomName = campaign.Title
			};
			await _groupUserChatRepository.CreateCampaignChatRoom(room);
		}

		public async Task<CampaignChatDTO> GetCampaignChatById(Guid campaignChatId)
        {
			var room = await _groupUserChatRepository.GetCampaignChatById(campaignChatId);
			var campaignChat = _mapper.Map<CampaignChatDTO>(room);
			return campaignChat;
		}
    }
}
