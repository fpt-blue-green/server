﻿
namespace BusinessObjects
{
    public class ChannelDTO
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public int? FollowersCount { get; set; }

        public int? ViewsCount { get; set; }

        public int? LikesCount { get; set; }

        public int? PostsCount { get; set; }

        public EPlatform Platform { get; set; }
    }

    public class ChannelStatDTO
    {
        public int? FollowersCount { get; set; }
        public int? ViewsCount { get; set; }
        public int? LikesCount { get; set; }
        public int? PostsCount { get; set; }
    }

    public class ChannelPlatFormUserNameDTO
    {
        public EPlatform Platform { get; set; }
        public string? UserName { get; set; }
    }
}
