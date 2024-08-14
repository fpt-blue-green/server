using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Tag
{
    public Guid Id { get; set; }

    public string? TagName { get; set; }

    public bool? IsPremiumTag { get; set; }

    public virtual ICollection<CampaignTag> CampaignTags { get; set; } = new List<CampaignTag>();

    public virtual ICollection<InfluencerTag> InfluencerTags { get; set; } = new List<InfluencerTag>();
}
