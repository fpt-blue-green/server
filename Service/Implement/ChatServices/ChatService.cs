using BusinessObjects.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _roomChatRepository = new ChatRepository();

        public async Task<List<ChatRoom>> GetMessagesAsync(Guid sender, Guid receiver)
        {
            return await _roomChatRepository.GetMessagesAsync(sender, receiver);
        }
        public async Task SaveMessageAsync(ChatRoom roomChat)
        {
            await _roomChatRepository.SaveMessageAsync(roomChat);
        }
    }
}
