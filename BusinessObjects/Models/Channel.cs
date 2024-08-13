using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Channel
{
    public Guid Id { get; set; }

    public Guid? InfluencerId { get; set; }

    public string? UserName { get; set; }

    public int? FollowersCount { get; set; }

    public int? ViewsCount { get; set; }

    public int? LikesCount { get; set; }

    public int? PostsCount { get; set; }

    public int? Type { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Influencer? Influencer { get; set; }
}
