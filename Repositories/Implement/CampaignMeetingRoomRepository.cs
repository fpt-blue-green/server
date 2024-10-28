using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class CampaignMeetingRoomRepository : ICampaignMeetingRoomRepository
    {
        public async Task CreateMeetingRoom(CampaignMeetingRoom campaignMeetingRoom)
        {
            using (var context = new PostgresContext())
            {
                context.CampaignMeetingRooms.Add(campaignMeetingRoom);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateMeetingRoom(CampaignMeetingRoom campaignMeetingRoom)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(campaignMeetingRoom).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteMeetingRoom(CampaignMeetingRoom campaignMeetingRoom)
        {
            using (var context = new PostgresContext())
            {
                context.CampaignMeetingRooms.Remove(campaignMeetingRoom);
                await context.SaveChangesAsync();
            }
        }

        public async Task<CampaignMeetingRoom> GetMeetingRoomByName(string name)
        {
            using (var context = new PostgresContext())
            {
                var roomMetting = await context.CampaignMeetingRooms.Include(m => m.Campaign).FirstOrDefaultAsync(r => r.RoomName == name);
                return roomMetting!;
            }
        }

        public async Task<CampaignMeetingRoom> GetMeetingRoomByCampaignId(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                var roomMetting = await context.CampaignMeetingRooms.FirstOrDefaultAsync(r => r.CampaignId == campaignId);
                return roomMetting!;
            }
        }
    }
}
