using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class MessageRepository : IMessageRepository
    {

        public async Task SaveMessageAsync(Message message)
        {
            using (var _context = new PostgresContext())
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Message>> GetMessagesAsync(Guid senderId, Guid receiverId)
        {
            using (var _context = new PostgresContext())
            {
                return await _context.Messages
                    .Include(s => s.Sender)
                    .Include(s => s.Receiver)
                .Where(c => c.SenderId == senderId && c.ReceiverId == receiverId
                || c.SenderId == receiverId && c.ReceiverId == senderId)
                .OrderBy(c => c.SentAt)
                .ToListAsync();
            }
        }
        public async Task<List<Message>> GetCampaignMessagesAsync(Guid senderId, Guid campaignChatId)
        {
            using (var _context = new PostgresContext())
            {
                return await _context.Messages
                    .Include(s => s.Sender)
                    .Include(s => s.Receiver)
                    .Include(s => s.CampaignChat)
                    .Where(c => c.CampaignChatId == campaignChatId)
                    .OrderBy(c => c.SentAt)
                    .ToListAsync();
            }
        }

        public async Task<Message> GetLastMessage(Guid senderId, Guid receiverId)
        {
            using (var _context = new PostgresContext())
            {
                return await _context.Messages
                    .Where(c =>
                        c.CampaignChatId == receiverId
                            ? (c.SenderId == senderId && c.CampaignChatId == receiverId)
                            : (c.SenderId == senderId && c.ReceiverId == receiverId) ||
                              (c.SenderId == receiverId && c.ReceiverId == senderId))
                    .OrderByDescending(c => c.SentAt)
                    .Select(m => new Message
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        CampaignChatId = m.CampaignChatId,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        ModifiedAt = m.ModifiedAt,
                        CampaignChat = m.CampaignChat == null ? null : new CampaignChat
                        {
                            Id = m.CampaignChat.Id,
                            RoomName = m.CampaignChat.RoomName
                        },
                        Receiver = m.Receiver == null ? null : new User
                        {
                            Id = m.Receiver.Id,
                            Email = m.Receiver.Email,
                            DisplayName = m.Receiver.DisplayName,
                            Avatar = m.Receiver.Avatar
                        },
                        Sender = new User
                        {
                            Id = m.Sender.Id,
                            Email = m.Sender.Email,
                            DisplayName = m.Sender.DisplayName,
                            Avatar = m.Sender.Avatar
                        }
                    })
                    .FirstOrDefaultAsync();
                    }
        }

        public async Task<List<Message>> GetMessagesByUserIdAsync(Guid userId)
        {
            using (var _context = new PostgresContext())
            {
                return await _context.Messages
                    .Where(c => c.SenderId == userId )
                    .OrderBy(c => c.SentAt)
                    .ToListAsync();
            }
        }
    }
}
