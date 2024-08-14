using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Package
{
    public Guid Id { get; set; }

    public Guid InfluencerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public bool? IsDisplay { get; set; }

    public int? Quantity { get; set; }

    public short? Status { get; set; }

    public virtual Influencer Influencer { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
