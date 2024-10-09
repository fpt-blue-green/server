using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Brand
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsPremium { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? CoverImg { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? FacebookUrl { get; set; }

    public string? TiktokUrl { get; set; }

    public string? YoutubeUrl { get; set; }

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual User User { get; set; } = null!;
}
