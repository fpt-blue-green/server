using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Tag
{
    public Guid Id { get; set; }

    public string? TagName { get; set; }

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Influencer> Influencers { get; set; } = new List<Influencer>();
}
