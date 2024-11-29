using BusinessObjects;
using BusinessObjects.Helper;
using BusinessObjects.Models;
using HtmlAgilityPack;
using Microsoft.IdentityModel.Tokens;
using Quartz.Util;
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
        private readonly ICampaignMeetingRoomRepository _campaignChatRepository = new CampaignMeetingRoomRepository();
        private readonly IChatMemberRepository _chatMemberRepository = new ChatMemberRepository();
        public async Task AddMemberToGroupChat(List<Guid> userIds, Guid campaignChatId)
        {
            foreach (var id in userIds)
            {
                var member = new ChatMember
                {
                    CampaignChatId = campaignChatId,
                    UserId = id,
                    JoinAt = DateTime.UtcNow,
                    Role = (int)EChatRole.Member
                };
                await _chatMemberRepository.AddNewMember(member);

            }
        }

        public async Task DeleteMember(Guid userId, Guid campaignChatId)
        {
            try
            {
                await _chatMemberRepository.DeleteMember(userId, campaignChatId);
            }
            catch
            {

            }
        }

        public async Task<ChatPartnerDTO> GetChatContactByIdAsync(Guid chatId)
        {
            var campaignChat = await _campaignChatRepository.GetMeetingRoomById(chatId);
            if (campaignChat != null)
            {
                return new ChatPartnerDTO
                {
                    ChatId = chatId,
                    ChatName = campaignChat.RoomName,
                    ChatImage = null,
                    isCampaign = true,
                    LastMessage = "",
                    SentAt = new DateTime(),
                    CampaignId = campaignChat.CampaignId,
                };
            }

            var user = await _userRepository.GetUserById(chatId);
            if (user != null)
            {

                return new ChatPartnerDTO
                {
                    ChatId = chatId,
                    ChatName = user.DisplayName,
                    ChatImage = user.Avatar,
                    isCampaign = false,
                    LastMessage = "",
                    SentAt = new DateTime()
                };
            }

            throw new KeyNotFoundException();
        }

        public async Task<List<ChatPartnerDTO>> GetChatContactsAsync(Guid userId, string? searchValue)
        {
            var messages = await _messageRepository.GetMessagesByUserIdAsync(userId);
            var campaignContacts = await _chatMemberRepository.GetMyGroupChat(userId);

            var userContacts = messages
                 .Where(m => !m.CampaignChatId.HasValue)
                 .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                 .Distinct()
                 .ToList();
            /*var campaignContacts = messages
                 .Where(m => m.CampaignChatId.HasValue)
                 .Select(m => m.CampaignChatId.Value)
                 .Distinct()
                 .ToList();
            campaignContacts.AddRange(campaignChats);*/
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

            foreach (var campaignChatId in campaignContacts.Distinct())
            {
                var campaign = await _campaignRepository.GetByCampaignChatId(campaignChatId);

                if (campaign != null)
                {
                    var lastMessage = messages
                        .Where(m => m.CampaignChatId == campaignChatId)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault();
                    if (lastMessage != null)
                    {
                        contacts.Add(new ChatPartnerDTO
                        {
                            ChatId = campaignChatId,
                            ChatName = campaign.Title,
                            SentAt = lastMessage?.SentAt,
                            LastMessage = lastMessage?.Content,
                            Sender = new UserDTO
                            {
                                Id = lastMessage.SenderId,
                                Email = lastMessage.Sender.Email,
                                Image = lastMessage.Sender.Avatar,
                                Name = lastMessage.Sender.DisplayName,
                                Role = (AuthEnumContainer.ERole)lastMessage.Sender.Role
                            },
                            isCampaign = true,
                            CampaignId = campaign.Id,
                        });
                    }
                    else
                    {
                        contacts.Add(new ChatPartnerDTO
                        {
                            ChatId = campaignChatId,
                            ChatName = campaign.Title,
                            isCampaign = true,
                            SentAt = DateTime.UtcNow,
                            CampaignId = campaign.Id,
                        });
                    }
                }
            }
            var result = contacts.OrderByDescending(c => c.SentAt).ToList();
            if (result.Count > 0 && !searchValue.IsNullOrWhiteSpace())
            {
                result = result.Where(s => TextComparator.ContainsIgnoreCaseAndDiacritics(s.ChatName, searchValue)).ToList();
            }
            return result;
        }
    }
}
