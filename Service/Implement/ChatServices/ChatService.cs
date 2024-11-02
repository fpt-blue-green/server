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
        private readonly IUserChatRepository _userUserChatRepository = new UserChatRepository();

        public async Task<List<UserChat>> GetMessagesAsync(Guid sender, Guid receiver)
        {
            return await _userUserChatRepository.GetMessagesAsync(sender, receiver);
        }
        public async Task SaveMessageAsync(UserChat userChat)
        {
            await _userUserChatRepository.SaveMessageAsync(userChat);
        }
    }
}
