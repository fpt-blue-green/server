using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class InfluencerTag
{
    public Guid Id { get; set; }

    public Guid? InfluencerId { get; set; }

    public Guid? TagId { get; set; }

    public virtual Influencer? Influencer { get; set; }

    public virtual Tag? Tag { get; set; }
}
