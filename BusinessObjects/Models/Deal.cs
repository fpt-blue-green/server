using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Deal
{
    public Guid Id { get; set; }

    public Guid InfluencerId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Influencer Influencer { get; set; } = null!;
}
