using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Favorite
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public Guid InfluencerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Influencer Influencer { get; set; } = null!;
}
