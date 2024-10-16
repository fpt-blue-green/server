using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CampaignContent
{
    public Guid Id { get; set; }

    public int Platform { get; set; }

    public int ContentType { get; set; }

    public int Quantity { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid CampaignId { get; set; }

    public decimal? Price { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;
}
