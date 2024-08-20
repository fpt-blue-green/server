using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ModelsDTO.InfluencerDTO
{
    public class ChannelDTO
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public int? FollowersCount { get; set; }

        public int? ViewsCount { get; set; }

        public int? LikesCount { get; set; }

        public int? PostsCount { get; set; }

        public int Type { get; set; }
    }
}
