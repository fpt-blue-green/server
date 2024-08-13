using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Offer
{
    public Guid Id { get; set; }

    public Guid? JobId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public int? From { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Job? Job { get; set; }
}
