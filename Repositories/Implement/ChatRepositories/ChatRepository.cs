using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ChatRepository : IChatRepository
    {

        public async Task SaveMessageAsync(ChatRoom roomChat)
        {
            using (var _context = new PostgresContext())
            {
                _context.ChatRooms.Add(roomChat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ChatRoom>> GetMessagesAsync(Guid senderId, Guid receiverId)
        {
            using (var _context = new PostgresContext())
            {
                return await _context.ChatRooms
                .Where(c => c.SenderId == senderId && c.ReceiverId == receiverId
                || c.SenderId == receiverId && c.ReceiverId == senderId)
                .OrderBy(c => c.DateSent)
                .ToListAsync();
            }
        }


    }
}
