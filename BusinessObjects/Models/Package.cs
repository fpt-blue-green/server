using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Package
{
    public Guid Id { get; set; }

    public Guid InfluencerId { get; set; }

    public int? Platform { get; set; }

    public int? ContentType { get; set; }

    public int? Duration { get; set; }

    public string Description { get; set; }

    public int? Price { get; set; }

    public int? Quantity { get; set; }

    public virtual Influencer Influencer { get; set; }
}
