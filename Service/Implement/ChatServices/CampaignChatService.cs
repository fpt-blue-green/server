
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using System.Transactions;

namespace Service
{
    public class CampaignChatService : ICampaignChatService
	{
		private readonly ICampaignChatRepository _groupUserChatRepository = new CampaignChatRepository();
        private readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private readonly IChatMemberRepository _chatMemberRepository = new ChatMemberRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<AutoMapperProfile>();
		}).CreateMapper();


		public async Task<CampaignChatDTO> CreateCampaignChatRoom(CampaignChatResDTO campaignChat, UserDTO brand)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var campaign = await _campaignRepository.GetById(campaignChat.CampaignId);
                    var room = new CampaignChat
                    {
                        CampaignId = campaignChat.CampaignId,
                        CreatedAt = DateTime.UtcNow,
                        RoomName = campaign.Title
                    };
                    var chatRoom = await _groupUserChatRepository.CreateCampaignChatRoom(room);
                    var chatAdmin = new ChatMember
                    {
                        CampaignChatId = chatRoom.Id,
                        UserId = brand.Id,
                        JoinAt = DateTime.UtcNow,
                        Role = (int)EChatRole.Admin
                    };
                    await _chatMemberRepository.AddNewMember(chatAdmin);
                    scope.Complete();
                    var chat = _mapper.Map<CampaignChatDTO>(chatRoom);
			        return chat; 
                }
                catch
                {
                    throw;
                }
            }
            
		}

		public async Task<CampaignChatDTO> GetCampaignChatById(Guid campaignChatId)
        {
			var room = await _groupUserChatRepository.GetCampaignChatById(campaignChatId);
			var campaignChat = _mapper.Map<CampaignChatDTO>(room);
			return campaignChat;
		}

		public async Task<CampaignChatDTO> GetCampaignChatByCampaignId(Guid campaignChatId)
        {
			var room = await _groupUserChatRepository.GetCampaignChatByCampaignId(campaignChatId);
			var campaignChat = _mapper.Map<CampaignChatDTO>(room);
			return campaignChat;
		}
    }
}
