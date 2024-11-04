namespace BusinessObjects
{
    public class CampaignMeetingRoomDTO
    {
        public Guid Id { get; set; }

        public string RoomName { get; set; } = null!;

        public DateTime StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public string? Participants { get; set; }

        public string? Description { get; set; }

        public bool IsFirstTime { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
