using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Feedback
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid InfluencerId { get; set; }

    public int? Rating { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Influencer Influencer { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
