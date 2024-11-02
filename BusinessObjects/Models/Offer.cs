using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Offer
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public int Platform { get; set; }

    public int ContentType { get; set; }

    public int? Duration { get; set; }

    public string? Description { get; set; }

    public int Price { get; set; }

    public int Status { get; set; }

    public int From { get; set; }

    public int TargetReaction { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? Quantity { get; set; }

    public virtual Job Job { get; set; } = null!;
}
