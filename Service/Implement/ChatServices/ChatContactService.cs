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

		public async Task<List<ChatPartnerDTO>> GetChatContactsAsync(Guid userId)
		{
			var messages = await _messageRepository.GetMessagesByUserIdAsync(userId);


			var userContacts = messages
				 .Where(m => !m.CampaignChatId.HasValue)
				 .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
				 .Distinct()
				 .ToList();
			var campaignContacts = messages
				 .Where(m => m.CampaignChatId.HasValue)
				 .Select(m => m.CampaignChatId.Value)
				 .Distinct()
				 .ToList();

			var contacts = new List<ChatPartnerDTO>();

			// Load thông tin của từng user contact
			foreach (var contactUserId in userContacts)
			{
				var user = await _userRepository.GetUserById(contactUserId.Value);

				if (user != null)
				{
					var lastMessage = messages
					.Where(m => (m.SenderId == contactUserId || m.ReceiverId == contactUserId) && !m.CampaignChatId.HasValue)
					.OrderByDescending(m => m.SentAt)
					.FirstOrDefault();
					Console.WriteLine(lastMessage.Sender.Id);
					contacts.Add(new ChatPartnerDTO
					{
						ChatId = user.Id,
						ChatName = user.DisplayName,
						ChatImage = user.Avatar,
						SentAt = lastMessage.SentAt,
						LastMessage = lastMessage.Content,
						Sender = new UserDTO
						{
							Id = lastMessage.SenderId,
							Email = lastMessage.Sender.Email,
							Image = lastMessage.Sender.Avatar,
							Name = lastMessage.Sender.DisplayName,
							Role = (AuthEnumContainer.ERole)lastMessage.Sender.Role
						},
						isCampaign = false
					}); ; ;
				}
			}

			foreach (var campaignChatId in campaignContacts)
			{
				var campaign = await _campaignRepository.GetByCampaignChatId(campaignChatId);

				if (campaign != null)
				{
					var lastMessage = messages
						.Where(m => m.CampaignChatId == campaignChatId)
						.OrderByDescending(m => m.SentAt)
						.FirstOrDefault();
					contacts.Add(new ChatPartnerDTO
					{
						ChatId = campaignChatId,
						ChatName = campaign.Title,
						SentAt = lastMessage.SentAt,
						LastMessage = lastMessage.Content,
						Sender = new UserDTO
						{
							Id = lastMessage.SenderId,
							Email = lastMessage.Sender.Email,
							Image = lastMessage.Sender.Avatar,
							Name = lastMessage.Sender.DisplayName,
							Role = (AuthEnumContainer.ERole)lastMessage.Sender.Role
						},
						isCampaign = true
					});
				}
			}

			return contacts.OrderByDescending(c => c.SentAt).ToList();
		}
	}
}
