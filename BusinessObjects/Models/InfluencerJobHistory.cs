using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class InfluencerJobHistory
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public Guid InfluencerId { get; set; }

    public Guid CampaignId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Status { get; set; }

    public DateTime? CompletionDate { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Influencer Influencer { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;
}
