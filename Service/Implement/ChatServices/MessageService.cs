using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _userUserChatRepository = new MessageRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task<List<MessageDTO>> GetCampaignMessagesAsync(Guid senderId, Guid campaignChatId)
        {
            var messages = await _userUserChatRepository.GetCampaignMessagesAsync(senderId, campaignChatId);
            var result = messages.Select(message => new MessageDTO
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                CampaignChatId = message.CampaignChatId,
                Content = message.Content,
                SentAt = message.SentAt,
                ModifiedAt = message.ModifiedAt,
                
                Receiver = message.Receiver == null ? null : new UserMessage
                {
                    Id = message.Receiver.Id,
                    Email = message.Receiver.Email,
                    Name = message.Receiver.DisplayName,
                    Image = message.Receiver.Avatar
                },
                Sender = new UserMessage
                {
                    Id = message.Sender.Id,
                    Email = message.Sender.Email,
                    Name = message.Sender.DisplayName,
                    Image = message.Sender.Avatar
                }
            }).ToList();
            return result;
        }
        public async Task SaveMessageAsync(MessageResDTO messageRes)
        {
            var message = new Message
            {
                ReceiverId = messageRes.ReceiverId,
                SenderId = messageRes.SenderId,
                CampaignChatId = messageRes.CampaignChatId,
                SentAt = DateTime.UtcNow,
                Content = messageRes.Content
            };
            await _userUserChatRepository.SaveMessageAsync(message);
        }
        public async Task<List<MessageDTO>> GetMessagesAsync(Guid senderId, Guid receiverId)
        {
            var message = await _userUserChatRepository.GetMessagesAsync(senderId, receiverId);
            return _mapper.Map<List<MessageDTO>>(message);
        }

        public async Task<MessageDTO> GetLastMessage(Guid senderId, Guid campaignChatId)
        {
            var message = await _userUserChatRepository.GetLastMessage(senderId, campaignChatId);

            if (message == null)
            {
                return null; 
            }

            try
            {
                var messageDTO = new MessageDTO
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    CampaignChatId = message.CampaignChatId,
                    Content = message.Content,
                    SentAt = message.SentAt,
                    ModifiedAt = message.ModifiedAt,
                    
                    Receiver = message.Receiver == null ? null : new UserMessage
                    {
                        Id = message.Receiver.Id,
                        Email = message.Receiver.Email,
                        Name = message.Receiver.DisplayName,
                        Image = message.Receiver.Avatar
                    },
                    Sender = new UserMessage
                    {
                        Id = message.Sender.Id,
                        Email = message.Sender.Email,
                        Name = message.Sender.DisplayName,
                        Image = message.Sender.Avatar
                    }
                };

                return messageDTO;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
