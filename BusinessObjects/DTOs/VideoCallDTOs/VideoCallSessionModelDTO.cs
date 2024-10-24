using Newtonsoft.Json;

namespace BusinessObjects
{
    public class VideoCallSessionModelDTO
    {
        public class ParticipantDTO
        {
            [JsonProperty("user_name")]
            public string UserName { get; set; }
            [JsonProperty("join_time")]
            public string JoinTime { get; set; }
            [JsonProperty("duration")]
            public int Duration { get; set; }
        }

        public class SessionDTO
        {
            [JsonProperty("room")]
            public string Room { get; set; }
            [JsonProperty("start_time")]
            public string StartTime { get; set; }
            [JsonProperty("duration")]
            public int Duration { get; set; }
            [JsonProperty("ongoing")]
            public bool Ongoing { get; set; }
            [JsonProperty("max_participants")]
            public int MaxParticipants { get; set; }
            [JsonProperty("participants")]
            public List<ParticipantDTO> Participants { get; set; }
        }

        public class DataSessionDTO
        {
            [JsonProperty("total_count")]
            public int TotalCount { get; set; }
            [JsonProperty("data")]
            public List<SessionDTO> Sessions { get; set; }
        }
    }
}
