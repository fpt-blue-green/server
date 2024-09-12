using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class InfluencerImage
{
    public Guid Id { get; set; }

    public Guid InfluencerId { get; set; }

    public string Url { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Influencer Influencer { get; set; } = null!;
}
