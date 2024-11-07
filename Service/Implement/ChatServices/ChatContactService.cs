using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChatContactService : IChatContactService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;

        public ChatContactService(IMessageRepository messageRepository, IUserRepository userRepository, ICampaignRepository campaignRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
        }

        public async Task<List<ChatContactDTO>> GetChatContactsAsync(Guid userId)
        {
            var messages = await _messageRepository.GetMessagesByUserIdAsync(userId);

            var userContacts = messages
                .Where(m => m.SenderId != userId || m.ReceiverId != userId) // Chỉ lấy người dùng khác
                .Select(m => new { m.SenderId, m.ReceiverId })
                .Distinct();

            var campaignContacts = messages
                .Where(m => m.CampaignChatId.HasValue) // Chỉ lấy tin nhắn có CampaignChatId
                .Select(m => m.CampaignChatId)
                .Distinct();

            var contacts = new List<ChatContactDTO>();

            foreach (var userContact in userContacts)
            {
                var contactUserId = userContact.SenderId == userId ? userContact.ReceiverId : userContact.SenderId;
                var user = await _userRepository.GetUserById((Guid)contactUserId);

                if (user != null)
                {
                    contacts.Add(new ChatContactDTO
                    {
                        UserId = user.Id,
                        UserName = user.DisplayName,
                        UserProfileImage = user.Avatar,
                        LastInteractionTime = messages
                            .Where(m => m.SenderId == contactUserId || m.ReceiverId == contactUserId)
                            .Max(m => m.SentAt)
                    });
                }
            }

            foreach (var campaignId in campaignContacts)
            {
                var campaign = await _campaignRepository.GetById(campaignId.Value);
                if (campaign != null)
                {
                    contacts.Add(new ChatContactDTO
                    {
                        CampaignId = campaign.Id,
                        CampaignName = campaign.Name,
                        LastInteractionTime = messages
                            .Where(m => m.CampaignChatId == campaignId)
                            .Max(m => m.SentAt)
                    });
                }
            }

            return contacts.OrderByDescending(c => c.LastInteractionTime).ToList();
        }
    }
}
