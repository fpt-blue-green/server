using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Job
{
    public Guid Id { get; set; }

    public Guid PackageId { get; set; }

    public Guid CampaignId { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual ICollection<InfluencerJobHistory> InfluencerJobHistories { get; set; } = new List<InfluencerJobHistory>();

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual Package Package { get; set; } = null!;
}
