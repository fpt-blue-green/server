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
		private readonly IMessageRepository _messageRepository = new MessageRepository();
		private readonly IUserRepository _userRepository = new UserRepository();
		private readonly ICampaignRepository _campaignRepository = new CampaignRepository();

		public async Task<List<ChatContactDTO>> GetChatContactsAsync(Guid userId)
		{
			var messages = await _messageRepository.GetMessagesByUserIdAsync(userId);


			var userContacts = messages
			   .Where(m => (m.SenderId == userId && m.ReceiverId != null) || m.ReceiverId == userId)
			   .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
			   .Distinct()
			   .ToList();
			var campaignContacts = messages
			   .Where(m => m.CampaignChatId.HasValue)
			   .Select(m => m.CampaignChatId.Value)
			   .Distinct()
			   .ToList();

			var contacts = new List<ChatContactDTO>();

			// Load thông tin của từng user contact
			foreach (var contactUserId in userContacts)
			{
				var user = await _userRepository.GetUserById(contactUserId.Value);

				if (user != null)
				{
					var lastInteractionTime = messages
						.Where(m => (m.SenderId == contactUserId || m.ReceiverId == contactUserId))
						.Max(m => m.SentAt);
					var lastMessage = messages
					.Where(m => (m.SenderId == contactUserId || m.ReceiverId == contactUserId))
					.OrderByDescending(m => m.SentAt).Select(s => s.Content)
					.FirstOrDefault();

					contacts.Add(new ChatContactDTO
					{
						UserId = user.Id,
						UserName = user.DisplayName,
						UserProfileImage = user.Avatar,
						LastInteractionTime = lastInteractionTime,
						LastMessage = lastMessage
					}); ;;
				}
			}

			foreach (var campaignChatId in campaignContacts)
			{
				var campaign = await _campaignRepository.GetByCampaignChatId(campaignChatId);

				if (campaign != null)
				{
					var lastInteractionTime = messages
						.Where(m => m.CampaignChatId == campaignChatId)
						.Max(m => m.SentAt);
					var lastMessage = messages
						.Where(m => m.CampaignChatId == campaignChatId)
						.OrderByDescending(m => m.SentAt).Select(s => s.Content)
						.FirstOrDefault();
					contacts.Add(new ChatContactDTO
					{
						CampaignId = campaign.Id,
						CampaignName = campaign.Title,
						LastInteractionTime = lastInteractionTime,
						LastMessage= lastMessage
					});
				}
			}

			return contacts.OrderByDescending(c => c.LastInteractionTime).ToList();
		}
	}
}
