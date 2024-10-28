using Newtonsoft.Json;

namespace BusinessObjects
{

    public class RoomSettingsDto
    {
        public string Name { get; set; }
        public RoomProperties Properties { get; set; }
        public string Privacy { get; set; }
    }

    public class RoomProperties
    {
        public long? Nbf { get; set; } = null!;
        public long? Exp { get; set; } = null!;

        [JsonProperty("eject_at_room_exp")]
        public bool EjectAtRoomExp { get; set; }

        public bool EnableKnocking { get; set; }
    }

    public class RoomDataRequest
    {
        public Guid? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string RoomName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<string> Participators { get; set; }
        public string Description { get; set; }
    }

    public class RoomDataUpdateRequest
    {
        public string RoomName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<string> Participators { get; set; }
        public string Description { get; set; }
    }
}
