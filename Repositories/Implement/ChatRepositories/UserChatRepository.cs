using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserChatRepository : IUserChatRepository
    {

        public async Task SaveMessageAsync(UserChat userChat)
        {
            using (var _context = new PostgresContext())
            {
                _context.UserChats.Add(userChat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserChat>> GetMessagesAsync(Guid senderId, Guid receiverId)
        {
            using (var _context = new PostgresContext())
            {
                return await _context.UserChats
                .Where(c => c.SenderId == senderId && c.ReceiverId == receiverId
                || c.SenderId == receiverId && c.ReceiverId == senderId)
                .OrderBy(c => c.DateSent)
                .ToListAsync();
            }
        }


    }
}
