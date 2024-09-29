using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class InfluencerReport
{
    public Guid Id { get; set; }

    public Guid ReporterId { get; set; }

    public Guid InfluencerId { get; set; }

    public int? Reason { get; set; }

    public string Description { get; set; } = null!;

    public int? ReportStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Influencer Influencer { get; set; } = null!;

    public virtual User Reporter { get; set; } = null!;
}
